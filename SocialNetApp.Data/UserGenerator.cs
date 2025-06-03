using System.Text;
using SocialNetApp.Data.Models;

namespace SocialNetApp.Data;

public static class UserGenerator
{
    private static readonly Random Random = new Random();
    private static readonly List<string> MaleNames =
    [
        "Александр", "Дмитрий", "Максим", "Сергей", "Андрей",
        "Алексей", "Иван", "Михаил", "Артем", "Владимир"
    ];

    private static readonly List<string> FemaleNames =
    [
        "Елена", "Ольга", "Наталья", "Анна", "Мария",
        "Ирина", "Екатерина", "Татьяна", "Светлана", "Юлия"
    ];

    private static readonly List<string> LastNames =
    [
        "Иванов", "Петров", "Сидоров", "Смирнов", "Кузнецов",
        "Васильев", "Попов", "Соколов", "Михайлов", "Новиков"
    ];
    private static readonly List<string> PartMiddleNames =
    [
        "Александр", "Дмитрий", "Сергей", "Андрей", "Алексей",
        "Игорь", "Олег", "Николай", "Владимир", "Виктор"
    ];
    private static string MiddleName(int index, bool isFemale)
    {
        return !isFemale ? $"{PartMiddleNames[index]}ович" : $"{PartMiddleNames[index]}овна";
    }
    public static List<User> GenerateUsers(int count)
    {
        var users = new List<User>();

        for (var i = 0; i < count; i++)
        {
            var isFemale = Random.Next(2) == 0;
            var firstName = isFemale
                ? FemaleNames[Random.Next(FemaleNames.Count)]
                : MaleNames[Random.Next(MaleNames.Count)];

            var lastName = LastNames[Random.Next(LastNames.Count)];
            var middleName = MiddleName(Random.Next(PartMiddleNames.Count), isFemale);

            if (isFemale)
            {
                lastName += "а";
            }

            var user = new User
            {
                UserName = GenerateUserName(firstName, lastName),
                Email = GenerateEmail(firstName, lastName),
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                BirthDate = GenerateBirthDate(),
                Image = isFemale
                    ? "https://avatar.iran.liara.run/public/girl"
                    : "https://avatar.iran.liara.run/public/boy",
                Status = GenerateStatus(),
                About = GenerateAbout(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            users.Add(user);
        }

        return users;
    }
    private static string ConvertToTranslit(string russianText)
    {
        if (string.IsNullOrEmpty(russianText))
            return russianText;

        var translitMap = new Dictionary<string, string>
        {
            {"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"}, {"е", "e"}, {"ё", "yo"},
            {"ж", "zh"}, {"з", "z"}, {"и", "i"}, {"й", "y"}, {"к", "k"}, {"л", "l"}, {"м", "m"},
            {"н", "n"}, {"о", "o"}, {"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"}, {"у", "u"},
            {"ф", "f"}, {"х", "kh"}, {"ц", "ts"}, {"ч", "ch"}, {"ш", "sh"}, {"щ", "shch"},
            {"ъ", ""}, {"ы", "y"}, {"ь", ""}, {"э", "e"}, {"ю", "yu"}, {"я", "ya"},
            {"А", "A"}, {"Б", "B"}, {"В", "V"}, {"Г", "G"}, {"Д", "D"}, {"Е", "E"}, {"Ё", "Yo"},
            {"Ж", "Zh"}, {"З", "Z"}, {"И", "I"}, {"Й", "Y"}, {"К", "K"}, {"Л", "L"}, {"М", "M"},
            {"Н", "N"}, {"О", "O"}, {"П", "P"}, {"Р", "R"}, {"С", "S"}, {"Т", "T"}, {"У", "U"},
            {"Ф", "F"}, {"Х", "Kh"}, {"Ц", "Ts"}, {"Ч", "Ch"}, {"Ш", "Sh"}, {"Щ", "Shch"},
            {"Ъ", ""}, {"Ы", "Y"}, {"Ь", ""}, {"Э", "E"}, {"Ю", "Yu"}, {"Я", "Ya"}
        };

        var result = new StringBuilder();
        foreach (var charStr in russianText.Select(c => c.ToString()))
        {
            result.Append(translitMap.GetValueOrDefault(charStr, charStr));
        }

        return result.ToString();
    }
    private static string GenerateUserName(string firstName, string lastName)
    {
        return ConvertToTranslit($"{firstName.ToLower()}.{lastName.ToLower()}");
    }

    private static string GenerateEmail(string firstName, string lastName)
    {
        return ConvertToTranslit($"{firstName.ToLower()}.{lastName.ToLower()}@example.com");
    }

    private static DateTime GenerateBirthDate()
    {
        var startDate = new DateTime(1950, 1, 1);
        var endDate = new DateTime(2005, 12, 31);
        var range = (endDate - startDate).Days;
        return startDate.AddDays(Random.Next(range));
    }

    private static string GenerateStatus()
    {
        var statuses = new List<string>
        {
            "Ура! Я в соцсети!",
            "Наслаждаюсь жизнью",
            "В поисках вдохновения",
            "Работаю",
            "Отдыхаю",
            "В путешествии"
        };
        return statuses[Random.Next(statuses.Count)];
    }

    private static string GenerateAbout()
    {
        var abouts = new List<string>
        {
            "Люблю путешествия и новые знакомства",
            "Увлекаюсь программированием",
            "Профессиональный фотограф",
            "Студент",
            "Работаю в IT",
            "Люблю читать книги"
        };
        return abouts[Random.Next(abouts.Count)];
    }
}