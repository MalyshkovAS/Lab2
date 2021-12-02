using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2
{
    //Точка входа
    class Program
    {
        static readonly GenethicAlgorythm genAlgorythm = new();
        static void Main(string[] args)
        {
            genAlgorythm.MainFunction = (x) => { return x * x + 4; }; //целевая функция
            genAlgorythm.Init(); //инициализация популяции
            var result = genAlgorythm.Process();
            Console.WriteLine("Count of itteration: {0}", genAlgorythm.GenerationCount);
            Console.WriteLine("Result: x = {0}; y = {1}.", result.X, result.Y);
            Console.WriteLine("Press Enter to exit...");
            Console.ReadKey();
        }
    }

    //Особь
    public class Person : IComparable
    {
        public double X;
        public double Y;
        public static int Compare(Person x, Person y)
        {
            var dlt = x.Y - y.Y;
            if (dlt > 0)
                return 1;
            if (dlt < 0)
                return -1;
            return 0;
        }
        public int CompareTo(object obj)
        {
            var y = obj as Person;
            var dlt = Y - y.Y;
            if (dlt > 0)
                return 1;
            if (dlt < 0)
                return -1;
            return 0;
        }
    }

    //Реализация генетического алгоритма
    public class GenethicAlgorythm
    {
        readonly static int populationSize = 2000;          //размер популяции
        readonly static int itterationLimit = 10000;        //максимум итераций
        readonly static double mutationProbability = 0.5;   //вероятность мутации
        readonly static double generationGap = 0.3;     //порог отсечения популяции
        readonly static double crossingProbability = 0.8;   //вероятность скрещивания
        readonly static double epsilon = 0.0001;            //точность вычислений
        readonly Random rnd = new((int)DateTime.Now.Ticks); //генератор случайных чисел

        public int GenerationCount { get; private set; }    //итоговое число поколений
        public Func<double, double> MainFunction;           //функция
        public List<Person> Persons = new(populationSize);  //Популяция

        public void Init()
        {
            InitPopulation();
            Evaluation();
        }

        public void InitPopulation() //получение начальной популяции!
        {
            for (int i = 0; i < populationSize; i++)
            {
                var person = new Person()
                {
                    X = (rnd.NextDouble() - 0.5) * 100000
                };
                Persons.Add(person);
            }
        }

        public void Evaluation() //оценка популяции
        {
            foreach (var person in Persons)
            {
                person.Y = ExecuteFunction(person.X);
            }
            Persons.Sort();
        }

        public Person Process() //Процесс работы генетического алгоритма
        {
            int i = 0;
            for (; i < itterationLimit; i++)
            {
                Crossing();
                Mutation();
                Evaluation();
                Selection();
                if (Math.Abs(Persons.First().Y - Persons.Last().Y) <= epsilon)
                    break;
            }
            GenerationCount = i;
            return Persons.First();
        }

        public void Selection() //селекция
        {
            for (int i = (int)(Persons.Count * generationGap); i < Persons.Count; i++)
                Persons.RemoveAt(i);
        }

        public void Crossing() //скрещивание
        {
            var oldPopulationSize = Persons.Count;
            while (Persons.Count < populationSize)
            {
                int i = (int)(rnd.NextDouble() * oldPopulationSize); //Выбор первого родителя
                int j = (int)(rnd.NextDouble() * oldPopulationSize); //Выбор второго родителя
                if (crossingProbability > rnd.NextDouble())
                {
                    var Childs = GetChilds(Persons[i], Persons[j]);
                    Persons.Add(Childs.Item1);
                    Persons.Add(Childs.Item2);
                }
            }
        }

        public (Person, Person) GetChilds(Person x, Person y) //получение потомков
        {
            var lambda = rnd.NextDouble();
            var zx = new Person() { X = lambda * x.X + (1 - lambda) * y.X };
            var zy = new Person() { X = lambda * y.X + (1 - lambda) * x.X };
            return (zx, zy);
        }

        public void Mutation() //мутация
        {
            foreach (var person in Persons)
            {
                if (mutationProbability > rnd.NextDouble())
                {
                    person.X += (rnd.NextDouble() - 0.5) * 100;
                }
            }
        }

        private double ExecuteFunction(double x)
        {
            return MainFunction(x);
        }
    }
}