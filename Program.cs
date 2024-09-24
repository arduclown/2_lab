using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace _2_lab;

public interface IPerson
{
    string Name { get; }
    string Patronomic { get; }
    string LastName { get; }
    DateTime Date { get; }
    int Age { get; }
}
public class Student : IPerson
{
    public string Name { get; }
    public string Patronomic { get;}
    public string LastName { get;}
    public DateTime Date { get; }
    public int Course { get;}
    public int Group {get;}
    public float Score {get;}

    public Student (string lastname, string name, string patronomic, DateTime date, int course, int group, float score)
    {
        this.LastName = lastname;
        this.Name = name;
        this.Patronomic = patronomic;
        this.Date = date;
        this.Course = course;
        this.Group = group;
        this.Score = score;
        if (Date > DateTime.Now)
            throw new Exception("Человек еще не родился");
    }
    
    public int Age => (DateTime.Now.Year - Date.Year) - (DateTime.Now.DayOfYear < Date.DayOfYear ? 1 : 0);
    public static Student Parse(string s)
    {
        var array = s.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        return new Student(array[0], array[1], array[2], DateTime.ParseExact(array[3], "MM-dd-yyyy", CultureInfo.InvariantCulture), int.Parse(array[4]), int.Parse(array[5]), float.Parse(array[6]));
    }
    
    public override string ToString() => $"{LastName,-15} {Name,-15} {Patronomic,-15} {Date:MM-dd-yyyy} {Age,-3} {Course,-5} {Group,-10} {Score:F2}";
    public string ToFileString() => ToString().Replace($"{Age,-3} ", "");

}


public class Teacher : IPerson 
{
    public string LastName { get; }
    public string Name { get; }
    public string Patronomic { get; }
    public DateTime Date { get; }
    public string Department { get; }
    public float Experience { get; }
    public enum Position 
    {
        Аспирант, Доцент, Профессор, Старший_Преподаватель
    }
    public Position Post {get;}
    public Teacher (string lastname, string name, string patronomic, DateTime date, string department, float experience, Position post)
    {
        this.LastName = lastname;
        this.Name = name;
        this.Patronomic = patronomic;
        this.Date = date;
        this. Department= department;
        this.Experience = experience;
        this.Post = post;
        if (Date > DateTime.Now)
            throw new Exception("Человек еще не родился");
    }
    public int Age => (DateTime.Now.Year - Date.Year) - (DateTime.Now.DayOfYear < Date.DayOfYear ? 1 : 0);
    
    public static Teacher Parse(string s)
    {
        string[] values = s.Split(" ");
        Enum.TryParse(Convert.ToString(values[6]), out Position Post);
        return new Teacher(values[0], values[1], values[2], DateTime.ParseExact(values[3], "MM-dd-yyyy", CultureInfo.InvariantCulture), values[4], float.Parse(values[5]), Post);
    }
    public override string ToString() => $"{LastName,-15} {Name,-15} {Patronomic,-15} {Date:MM-dd-yyyy} {Age,-3} {Department,-6} {Experience,-10:F4} {Post}";
    public string ToFileString() => ToString().Replace($"{Age,-3} ", "");

}

public interface IUniversity
{
   IEnumerable<IPerson> Persons { get; }   // отсортировать в соответствии с вариантом 1-й лабы
   IEnumerable<Student> Students { get; }  // отсортировать в соответствии с вариантом 1-й лабы
   IEnumerable<Teacher> Teachers { get; }  // отсортировать в соответствии с вариантом 1-й лабы

   void Add(IPerson person);
   void Remove(IPerson person);

   IEnumerable<IPerson> FindByLastName(string lastName);

   // Для четных вариантов. Выдать всех преподавателей, название кафедры которых содержит
   // заданный текст. Отсортировать по должности.
   IEnumerable<Teacher> FindByDepartment(string text);
}


public class University : IUniversity
{
    List<IPerson> persons = new List<IPerson>();
    public IEnumerable<IPerson> Persons => persons.OrderBy(p => p.Date);// Сортировка всех людей по дате рождения
    public IEnumerable<Student> Students => persons.OfType<Student>().OrderBy(p => p.Date);// Сортировка студентов  по дате рождения
    public IEnumerable<Teacher> Teachers => persons.OfType<Teacher>().OrderBy(p => p.Date);// Сортировка преподавателей  по дате рождения

    public void Add(IPerson person) 
    {
        persons.Add(person);
    }

    public void Remove(IPerson person) 
    {
        persons.Remove(person);
    } 

    public IEnumerable<IPerson> FindByLastName(string lastName) => persons.Where(x => x.LastName == lastName);

    public IEnumerable<Teacher> FindByDepartment(string text) => persons.OfType<Teacher>().Where(x => x.Department.Contains(text)).OrderBy(x => x.Post);
}

class Programm
{
    static University university = new University();
    static void Main()
    {
        if (!File.Exists("Students.txt"))
        {
            Console.WriteLine("Файл не найден.");
            return;
        }

        string[] Students = File.ReadAllLines("Students.txt");
        foreach (string s in Students)
        {
            university.Add(Student.Parse(s));
        }
        if (!File.Exists("Teachers.txt"))
        {
            Console.WriteLine("Файл не найден.");
            return;
        }
        string[] Teachers = File.ReadAllLines("Teachers.txt");
        foreach (string s in Teachers)
        {
            university.Add(Teacher.Parse(s));
        }
        Console.WriteLine("1) Добавить студента");
        Console.WriteLine("2) Добавить преподавателя");
        Console.WriteLine("3) Удалить человека из базы");
        Console.WriteLine("4) Поиск по фамилии");
        Console.WriteLine("5) Поиск по кафедре");
        Console.WriteLine("6) Вывести список студентов");
        Console.WriteLine("7) Вывести список преподавателей");
        Console.WriteLine("8) Сохранить базу данных");
        Console.WriteLine("9) Выход");

        int number = int.Parse(Console.ReadLine());

        while (true) {

        switch(number)
        {
            case 1: 
                Console.WriteLine("Введите данные студента: Фамилия, Имя, Отчество, Дата рождения, Курс, Группа, Средний балл");
                string student = Console.ReadLine();
                university.Add(Student.Parse(student));
                Console.WriteLine($"Добавлен студент: {student}");
                break;
            case 2:
                Console.WriteLine("Введите данные перподавателя: Фамилия, Имя, Отчество, Дата рождения, Кафедра, Стаж, Должность");
                string teacher = Console.ReadLine();
                university.Add(Teacher.Parse(teacher));
                Console.WriteLine($"Добавлен преподаватель: {teacher}" );
                break;
            case 3:
                RemovePerson();
                break;
            case 4:
                FindByLastName();
                break;
            case 5:
                FindByDepartment();
                break;
            case 6:
                PrintStudents();
                break;
            case 7:
                PrintTeachers();
                break;
            case 8:
                SaveStudents();
                SaveTeachers();
                break;
            case 9:
                return;
            default: 
                Console.WriteLine("Некорректные данные. Выберите цифру от 1 до 8");
                break;

        }
        number = int.Parse(Console.ReadLine());

        }

        static void FindByLastName ()
        {
            Console.WriteLine("Введите фамилию:");
            string lastname = Console.ReadLine();
            var persons = university.FindByLastName(lastname);
            if (persons.Any()) 
            {
                foreach(var people in persons)
                    Console.WriteLine(people.ToString());
            }
            else
            {
                Console.WriteLine("Люди с такими фамилиями не найдены");
                
            }
        }

        static void RemovePerson()
        {
            Console.WriteLine("Введите фамилию:");
            string lastname = Console.ReadLine();
            var person = university.FindByLastName(lastname);
            
            if (person.Any())
            {
                if (person.Count() == 1)
                {
                    university.Remove(person.First());
                    Console.WriteLine("Удалён");
                }
                else
                {
                    Console.WriteLine("Найдено несколько человек с такой фамилией:");
                    foreach(var people in person)
                        Console.WriteLine(people.ToString());
                    Console.WriteLine("Выберите человека. Введите имя:");
                    string firstname = Console.ReadLine();
                    var personToRemove = person.FirstOrDefault(x => x.Name == firstname);
                    
                    if(personToRemove != null)
                    {
                        university.Remove(personToRemove);
                        Console.WriteLine("Удалён");
                    }
                    else
                    {
                        Console.WriteLine("Человек не найден");
                    }
                }
                
            }
            else
            {
                Console.WriteLine("Человек не найден");
            }
        }


        static void FindByDepartment()
        {
            Console.WriteLine("Введите кафедру:");
            string department = Console.ReadLine();
            var sub = university.FindByDepartment(department);
            if (!sub.Any()) 
                Console.WriteLine("Кафедра не найдена");
            else
            {
                foreach(var people in sub)
                    Console.WriteLine(people.ToString());
            }
        }

        static void PrintStudents()
        {
            Console.WriteLine("Список студентов:");
            foreach(var students in university.Students)
            {
                Console.WriteLine(students.ToString());
            }
        }

        static void PrintTeachers()
        {
            Console.WriteLine("Список преподавателей:");
            foreach(var teachers in university.Teachers)
            {
                Console.WriteLine(teachers.ToString());
            }
        }

        static void SaveStudents()
        {
            List<string> output = new List<string>();
            foreach(var people in university.Students)
                output.Add(people.ToFileString());
            File.WriteAllLines("Students.txt", output);
        }

        static void SaveTeachers()
        {
            List<string> output = new List<string>();
            foreach(var people in university.Teachers)
                output.Add(people.ToFileString());
            File.WriteAllLines("Teachers.txt", output);
        }
    }

}