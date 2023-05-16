using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Bok
{
    public string Titel { get; private set; }
    public string Författare { get; private set; }
    public string Ämne { get; private set; }
    public string ISBN { get; private set; }

    public Bok(string titel, string författare, string ämne, string isbn)
    {
        Titel = titel;
        Författare = författare;
        Ämne = ämne;
        ISBN = isbn;
    }
}


class User
{
    public string Personnummer { get; private set; }
    public string Lösenord { get; set; }
    public string FörNamn { get; private set; }
    public string EfterNamn { get; private set; }

    public User(string personnummer, string lösenord, string firstName, string lastName) // Update the constructor
    {
        Personnummer = personnummer;
        Lösenord = lösenord;
        FörNamn = firstName;
        EfterNamn = lastName;
    }
}
class Program
{

    static List<User> users = new List<User>();

    static List<Bok> böcker;

    static void SaveUsersToFile()
    {
        using (StreamWriter sw = new StreamWriter("users.txt"))
        {
            foreach (User user in users)
            {
                sw.WriteLine($"{user.Personnummer}:{user.FörNamn}:{user.EfterNamn}:{user.Lösenord}");
            }
        }
    }

    static void LoadUsersFromFile()
    {
        users = new List<User>();

        if (File.Exists("users.txt"))
        {
            using (StreamReader sr = new StreamReader("users.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 4)
                    {
                        users.Add(new User(parts[0], parts[3], parts[1], parts[2]));
                    }
                }
            }
        }
    }
    private static void InitializeBooks()
    {
        böcker = new List<Bok>()
    {
        new Bok("Harry Potter och De Vises Sten", "J.K. Rowling", "Fantasy", "988122514142"),
        new Bok("Gösta Berglings Saga", "Selma lagerllöf", "Roman", "982094818321"),
        new Bok("Bamse och hans vänner", "Jan Magnusson", "Lek&pyssel", "90081248029")
    };

    }


    static Bok SökBok()
    {
        Console.WriteLine("\nSkriv in sökordet:");
        string sökord = Console.ReadLine().ToLower();

        if (böcker == null)
        {
            Console.WriteLine("Boklistan är tom, var god och initiera böcker först.");
            return null;
        }

        List<Bok> resultat;

        try
        {
            resultat = böcker.Where(bok => bok.Titel.ToLower().Contains(sökord) ||
                                           bok.Författare.ToLower().Contains(sökord) ||
                                           bok.Ämne.ToLower().Contains(sökord) ||
                                           bok.ISBN.ToLower().Contains(sökord)).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ett fel uppstod under sökningen: " + ex.Message);
            return null;
        }

        if (resultat.Count > 0)
        {
            Console.WriteLine("\nDetta kom fram:");
            foreach (Bok bok in resultat)
            {
                Console.WriteLine("- " + bok.Titel + " av " + bok.Författare);
            }

            Console.WriteLine("\nVilken bok vill du låna?");
            string valdTitel = Console.ReadLine();

            Bok valdBok = resultat.Find(bok => bok.Titel.Equals(valdTitel, StringComparison.CurrentCultureIgnoreCase)); //Frågar efter bok - Viktigt att bara skriva titeln av bok man vill låna
            if (valdBok != null)
            {
                böcker.Remove(valdBok); // Lämnar tillbaka bok
                return valdBok;
            }
            else
            {
                Console.WriteLine("Denna bok fanns inte i biblioteket.");
            }
        }
        else
        {
            Console.WriteLine("\nTyvärr men din sökning gav inga resultat.");
        }

        return null;
    }





    static void lånabok(Bok borrowedBok)
    {
        while (true)
        {
            Console.WriteLine("\nVad vill du göra?");
            Console.WriteLine("1. Lämna tillbaka boken");
            Console.WriteLine("2. Logga ut");

            Console.Write("Skriv ditt val (1-2): ");
            string val = Console.ReadLine();

            if (val == "1")
            {
                böcker.Add(borrowedBok); // Lägger tillbaka ´den lånade boken
                Console.WriteLine("Du har lämnat tillbaka boken.");
                break;
            }
            else if (val == "2")
            {
                Console.WriteLine("Utloggad.");
                break;
            }
            else
            {
                Console.WriteLine("Ogiltigt val. Försök igen");
            }
        }
    }


    static void Main(string[] args)
    {
        Console.WriteLine("Välkommen!");
        InitializeBooks();
        LoadUsersFromFile();

        while (true)
        {
            Console.WriteLine("\nVad vill du göra?");
            Console.WriteLine("1. Skapa ett konto");
            Console.WriteLine("2. Logga in");
            Console.WriteLine("3. Avsluta");

            Console.Write("Skriv ditt val (1-3): ");
            string val = Console.ReadLine();

            if (val == "1")
            {
                SkapaKonto();
            }
            else if (val == "2")
            {
                User loggedInUser = LoggaIn();
                if (loggedInUser != null)
                {
                    Console.WriteLine("Välkommen till ditt konto!");
                    LoggedInMenu(loggedInUser);
                }
                else
                {
                    Console.WriteLine("Fel personnummer eller lösenord. Försök igen.");
                }
            }
            else if (val == "3")
            {
                Console.WriteLine("Tack för besöket! Hej då.");
                break;
            }
            else
            {
                Console.WriteLine("Ogiltigt val. Försök igen.");
            }
        }
    }

    static void LoggedInMenu(User user)
    {
        while (true)
        {
            Console.WriteLine("\nVad vill du göra?");
            Console.WriteLine("1. Sök efter bok");
            Console.WriteLine("2. Logga ut");
            Console.WriteLine("3. Ändra lösenord");

            Console.Write("Skriv ditt val (1-3): ");
            string val = Console.ReadLine();

            if (val == "1")
            {
                Bok lånatBok = SökBok();
                if (lånatBok != null)
                {
                    Console.WriteLine("Du har lånat boken: " + lånatBok.Titel + " av " + lånatBok.Författare);
                    lånabok(lånatBok);
                }
            }
            else if (val == "2")
            {
                Console.WriteLine("Du har loggat ut.");
                break;
            }
            else if (val == "3")
            {
                AndraLosenord(user);
            }
            else
            {
                Console.WriteLine("Ogiltigt val. Försök igen.");
            }
        }
    }

    static void SkapaKonto()
    {
        Console.WriteLine("\nFör att skapa ett konto, vänligen ange ditt personnummer, förnamn, efternamn och ett lösenord.");

        Console.Write("Personnummer: ");
        string personnummer = Console.ReadLine();

        Console.Write("Förnamn: ");
        string firstName = Console.ReadLine();

        Console.Write("Efternamn: ");
        string lastName = Console.ReadLine();

        Console.Write("Lösenord: ");
        string lösenord = Console.ReadLine();

        if (!users.Any(u => u.Personnummer == personnummer))
        {
            users.Add(new User(personnummer, lösenord, firstName, lastName));
            SaveUsersToFile();
            Console.WriteLine("Ditt konto har skapats. Tack!");
        }
        else
        {
            Console.WriteLine("Detta personnummer är redan registrerat. Försök igen med ett annat personnummer.");
        }
    }
    static User LoggaIn()
    {
        Console.WriteLine("\nFör att logga in, vänligen ange ditt personnummer och lösenord.");

        Console.Write("Personnummer: ");
        string personnummer = Console.ReadLine();

        Console.Write("Lösenord: ");
        string lösenord = Console.ReadLine();

        var user = users.FirstOrDefault(u => u.Personnummer == personnummer && u.Lösenord == lösenord);

        if (user != null)
        {
            return user; // Return the user if login is successful
        }
        return null; // Return null if the login fails
    }

    static void AndraLosenord(User user)
    {
        Console.WriteLine("\nFör att ändra ditt lösenord, vänligen ange ditt nuvarande lösenord.");

        Console.Write("Nuvarande lösenord: ");
        string nuvarandeLosenord = Console.ReadLine();

        if (user.Lösenord == nuvarandeLosenord)
        {
            Console.Write("Nytt lösenord: ");
            string nyttLosenord = Console.ReadLine();

            user.Lösenord = nyttLosenord;
            SaveUsersToFile();
            Console.WriteLine("Ditt lösenord har uppdaterats. Tack!");
        }
        else
        {
            Console.WriteLine("Ogiltigt lösenord. Försök igen.");
        }
    }
}


