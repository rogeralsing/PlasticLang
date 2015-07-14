using System;

namespace PlasticLangLabb1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var code = @"

let a = 1
let b = 3
let c = a + b
a = a + 1
print('c = ' + c)
print('a = ' + a)

let arr  = ['hello','this','is','an','array']

print ('arr length is ' + (arr.'Length' + 100) )
print ('str length is ' + 'some string'.Length)

let closureprint = x => print(x + a)
closureprint('foo')

each(element, arr)
{
    print(element)
}

for (a = 0; a < 10; a ++) 
{
    print (a);
}

repeat(3)
{
    print('repeat..')
}

if (a == 1)
{
    print ('inside if')
}
elif (a == 3)
{
    print ('inside elif')
}
else
{
    print ('inside else')
}

while (a < 20)
{
     print ('daisy me rollin`')
     a++
};

(x => print('lambda fun ' + x))('yay')

if (true, print('hello'));

let f = func(a,b,c) 
{
    print('abc '+a+' '+b+' '+c);
}

f(1)(2)(3);


";
            Plastic.Run(code);
            Console.ReadLine();
        }
    }
}


/*

let arr = [0, 'hello']
let obj = [a:0, b:'hello']
obj['a'] 

match (obj; [a[c],b]; print (c))

*/