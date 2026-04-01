var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

Repository repo = new Repository();
app.MapPost("/create", (string category, string mass, float price, string stranaOrJanr) =>
{
    try { repo.Create(category, mass, price, stranaOrJanr); }
    catch (Exception ex) { throw new Exception(ex.Message); }

});
app.MapGet("/read", repo.Read);
app.MapGet("/readTo/{id}", (Guid id) => repo.ReadTo(id));
app.MapGet("/filter/{category}", (string category) =>
{
    try { repo.FilterSpisok(category); }
    catch (Exception ex) { throw new Exception(ex.Message); }
});
app.MapGet("/search/{search}", (string search) => repo.Search(search));
app.MapPut("/update/{id}/{name}/{price}", (Guid id, string name, float price) => { repo.Update(id, name, price); });
app.MapDelete("/delate/{id}", (Guid id) => { repo.Delete(id); });

app.Run();


/// <summary>
/// Класс для взаимодейвствия со списком
/// </summary>
public class Repository
{
    private List<Tovar> Spisok = new List<Tovar>();
    /// <summary>
    /// Создание объекта в списке
    /// </summary>
    /// <param name="category">Название категории товара</param>
    /// <param name="mass">Название товара</param>
    /// <param name="price">Цена товара</param>
    public Tovar Create(string category, string mass, float price, string stranaOrJanr)
    {
        switch (category)
        {
            case "ВПН":
                string strana = stranaOrJanr;
                VPN vpn = new VPN(mass, "ВПН", price, strana);
                Spisok.Add(vpn);
                return vpn;
            case "Игра":
                string janr = stranaOrJanr;
                Game game = new Game(mass, "Игры", price, janr);
                Spisok.Add(game);
                return game;
            default:
                throw new Exception("Товарищ, такой категории у нас в магазине — нет");

        }
    }
    /// <summary>
    /// Список товаров
    /// </summary>
    /// <returns>Возвращает полный список товаров в списке</returns>
    public List<Tovar> Read()
    {
        return Spisok;
    }
    /// <summary>
    /// Информация о товаре по идентификатору
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <returns>Возвращает информацию о единичном товаре из списка по идентификатору</returns>
    public Tovar ReadTo(Guid id)
    {
        for (int i = 0; i < Spisok.Count; i++)
        {
            if (Spisok[i].Id == id) { return Spisok[i]; }
        }
        return null;

    }
    /// <summary>
    /// Обновление информации о товаре
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <param name="mass">Название товара</param>
    /// <param name="price">Цена товара</param>
    public void Update(Guid id, string mass = "", float price = 0)
    {
        for (int i = 0; i < Spisok.Count; i++)
        {
            if (Spisok[i].Id == id)
            {
                if (mass != "") { Spisok[i].Name = mass; }
                if (price != 0) { Spisok[i].Price = price; }
                break;
            }
        }
    }
    /// <summary>
    /// Удаление товара из списка по идентифифкатору
    /// </summary>
    /// <param name="id">идентификатор</param>
    public void Delete(Guid id)
    {
        try
        {
            for (int i = 0; i < Spisok.Count; i++)
            {
                if (Spisok[i].Id == id) { Spisok[i] = null; Spisok.Remove(Spisok[i]); }
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

    }
    /// <summary>
    /// Фильтрация списка товаров по категории
    /// </summary>
    /// <param name="category">параметр категории по которму фильтруется</param>
    /// <returns>Возвращает отфильтрованный список</returns>
    public List<Tovar> FilterSpisok(string category)
    {
        // try
        //{
        List<Tovar> buffer = Spisok.Where(Sp => Sp.Category == category).ToList();
        return buffer;
        //}
        //catch (Exception ex) { Console.WriteLine(ex.Message); }
        //return null;
    }
    /// <summary>
    ///Нестрогий поиск по параметрам
    /// </summary>
    /// <param name="request">поисковой запрос</param>
    /// <returns>Возвращает найденные товары</returns>
    public List<Tovar> Search(string request)
    {
        List<Tovar> buffer = new List<Tovar>();
        for (int i = 0; i < Spisok.Count; i++)
        {
            if (Convert.ToString(Spisok[i].Id).ToLower().Contains(request.ToLower())) { buffer.Add(Spisok[i]); }
            else if (Spisok[i].Category.ToLower().Contains(request.ToLower())) { buffer.Add(Spisok[i]); }
            else if (Spisok[i].Name.ToLower().Contains(request.ToLower())) { buffer.Add(Spisok[i]); }
            else if (Convert.ToString(Spisok[i].Price).Contains(request)) { buffer.Add(Spisok[i]); }
        }
        if (buffer.Count == 0) { throw new Exception("Ничего не найдено"); }
        return buffer;
    }
}

/// <summary>
/// Класс для категории впн в магазине
/// </summary>
public class VPN : Tovar
{
    /// <summary>
    /// Страна местонахождения впн
    /// </summary>
    public string Strana { get; set; }

    /// <summary>
    /// Конструктор для инициализации объекта из категории впн
    /// </summary>
    /// <param name="name"> имя</param>
    /// <param name="price"> цена </param>
    /// <param name="strana"> страна</param>
    public VPN(string name, string category, float price, string strana) : base(name, category, price)
    {
        Strana = strana;
    }
    /// <summary>
    /// Метод конвертации в строку
    /// </summary>
    /// <returns>возвращает объект из категории впн сконвертированным в строку</returns>
    public override string ToString()
    {
        return base.ToString() + $" страна: {Strana}";
    }
}

/// <summary>
/// Класс для категории игр в магазине
/// </summary>
public class Game : Tovar
{
    /// <summary>
    /// Жанр игры
    /// </summary>
    public string Janr { get; set; }

    /// <summary>
    /// Конструктор для категории игр
    /// </summary>
    /// <param name="name">имя игры</param>
    /// <param name="price">цена игры</param>
    /// <param name="janr">жанр игры</param>
    public Game(string name, string category, float price, string janr) : base(name, category, price)
    {
        Janr = janr;
    }
    /// <summary>
    /// Метод конвертации в строку
    /// </summary>
    /// <returns>возвращает объект конвертированный в строку</returns>
    public override string ToString()
    {
        return base.ToString() + $" жанр: {Janr}";
    }

}

/// <summary>
/// Основной класс товара от которого будут наследоватся все товары из любых категорий
/// </summary>
public class Tovar
{
    /// <summary>
    /// Свойство идентификатор
    /// </summary>
    public Guid Id { get; }
    /// <summary>
    /// Имя товара
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Цена товара
    /// </summary>
    private float price;
    /// <summary>
    /// Свойство цены товара с валидацией значения
    /// </summary>
    public float Price
    {
        get { return price; }
        set
        {
            if (value > 0) { price = value; }
            else { throw new Exception("Нельзя ввести отрицательную стоимость"); }
        }
    }
    /// <summary>
    /// Нзвание категории в которой товар находится
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Конструктор класса для инициализации товара
    /// </summary>
    /// <param name="name">имя товара</param>>
    /// <param name="category">категория товара в которой он находтся</param>
    /// <param name="price">цена товара</param>>
    public Tovar(string name, string category, float price)
    {
        Name = name;
        Price = price;
        Category = category;
        Id = Guid.NewGuid();
        Console.WriteLine($"Создан товар с идентификатором: {Id} ");
    }
    /// <summary>
    /// Конвертация в строку
    /// </summary>
    /// <returns>возваращает полностью товар конвертированный в строку</returns>
    public override string ToString()
    {
        return $"{Id}. {Name} категория: {Category} цена: {Price}";
    }
}
