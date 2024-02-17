using MathNet.Numerics.Distributions;
using Max.BaseKit.Customs;
using Max.BaseKit.Utils;
using System.Collections.Concurrent;

internal class Program
{
    private static void Main(string[] args)
    {
        Test2();
        Console.WriteLine("Hello, World!");
        Console.ReadKey(true);
    }

    private static void Test()
    {
        //List<int> ints1 = new List<int>();
        //for (int i = 1;i<=200;i++)
        //{
        //    ints1.Add(i);
        //}
        //foreach (var i in ints1)
        //{
        //    if (i == 18)
        //        break;
        //}
        //ints1?.ForEach(i =>
        //{
        //    if (i == 18) 
        //        return;
        //});

        string input = "  123  abc 123  ";
        input = input.TrimStart(' ');
        var ll = input.TrimStart("1".ToCharArray());


        TestEnum tEnum0 = (TestEnum)0;
        TestEnum tEnum1 = (TestEnum)1;
        TestEnum tEnum2 = (TestEnum)2;
        TestEnum tEnum3 = (TestEnum)3;
        TestEnum tEnum4 = (TestEnum)4;


        List<string> strings = new List<string>()
        {
        "1@2@20230101234556333_XXX.txt",
        "0@3@20220101234556333_XXX.txt",
        "1@1@20240101234556333_XXX.txt",
        "0@1@20250101234556333_XXX.txt"
        };
        strings = strings.OrderBy(x => x).ToList();

        //ConcurrentBag<string> strings1 = new ConcurrentBag<string>()
        //{
        //"1@2@20230101234556333_XXX.txt",
        //"0@3@20220101234556333_XXX.txt",
        //"1@1@20240101234556333_XXX.txt",
        //"0@1@20250101234556333_XXX.txt"
        //};
        //var llll = strings1.OrderBy(x => x).FirstOrDefault();

        var files = FileUtil.FindFiles(new DirectoryInfo(@"D:\Test2\"), null, "*.png");
        files?.ForEach(f =>
        {
            var l1 = f.Name;//001.png
            var l2 = f.FullName;//D:\Test2\001.png
        });

        string sourceFilePath = @"D:\Test1\001.png";
        string destinationFilePath = @"D:\Test2\001.png";
        using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                sourceStream.CopyTo(destinationStream);
            }
        }
        using (FileStream fileStream = new FileStream(sourceFilePath, FileMode.Truncate, FileAccess.Write))
        {
            fileStream.SetLength(0);
        }

        //File.Delete(sourceFilePath);
        Console.WriteLine("文件移动成功！");


        BlockingCollection<int> ints = new BlockingCollection<int>(20);
        Task.Factory.StartNew(() =>
        {
            int i = 1;
            while (i < 200)
            {
                bool flag = ints.TryAdd(i);
                Console.WriteLine($"1号插入{i}：{flag}");
                i++;
                Thread.Sleep(100);
            }
        });
        Task.Factory.StartNew(() =>
        {
            int i = 1;
            while (i < 200)
            {
                bool flag = ints.TryAdd(i);
                Console.WriteLine($"2号插入{i}：{flag}");
                i++;
                Thread.Sleep(100);
            }
        });
        Task.Factory.StartNew(() =>
        {
            foreach (int item in ints.GetConsumingEnumerable())
            {
                Thread.Sleep(150);
                Console.WriteLine($"{item}    数量：{ints.Count}");
            }
        });
    }

    static ConcurrentDictionary<Type, CustomConcurrentList<object>> dic = new ConcurrentDictionary<Type, CustomConcurrentList<object>>();

    private static void Test2()
    {
        string str = "123,234";
        var dbidArry = str? .Split(',');
        if (dbidArry == null || dbidArry.Length > 1)
        {

        }
        else
        { 
        
        }


        Insert(new Person1() { Name = "Alice", Age = 12 });
        Insert(new Person2() { Name = "Bob", Age = 13 });

        //var people1 = new List<Person1>()
        //{
        //    new Person1() { Name = "Alice",Age=12 },
        //    new Person1() { Name = "Bob" ,Age=13},
        //    new Person1() { Name = "Charlie",Age=14 }
        //};
        //var people2 = new List<Person2>()
        //{
        //    new Person2() { Name = "Alice",Age=12 },
        //    new Person2() { Name = "Bob" ,Age=13},
        //    new Person2() { Name = "Charlie",Age=14 }
        //};
    }

    private static void Insert(object obj)
    {
        if (!dic.ContainsKey(obj.GetType()))
        {
            dic.TryAdd(obj.GetType(), new CustomConcurrentList<object>());
        }

        dic.TryGetValue(obj.GetType(), out var entity);
        entity?.Add(obj);
    }
}
public enum TestEnum
{
    Test1, test2, test3
}

public class Person1
{
    public string Name { get; set; }
    public int Age { get; set; }
}
public class Person2
{
    public string Name { get; set; }
    public int Age { get; set; }
}