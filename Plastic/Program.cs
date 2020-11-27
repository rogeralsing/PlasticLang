using System;
using System.IO;

namespace PlasticLang
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var code = File.ReadAllText("sample.pla");
            Plastic.Run(code);
            Console.ReadLine();
            //var result = Parser.Default.ParseArguments<Options>(args);
            //var returnvalue = result.Return(options =>
            //{
            //    var code = File.ReadAllText("sample.pla");
            //    Plastic.Run(code);
            //    return 0;
            //},error => 1);
            //Console.ReadLine();
            //return returnvalue;
        }
    }
}

/*

let arr = [0, 'hello']
let obj = [a:0, b:'hello']
obj['a'] 

match (obj; [a[c],b]; print (c))

*/