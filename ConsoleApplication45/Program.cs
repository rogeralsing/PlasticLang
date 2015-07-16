using System;

namespace PlasticLang
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var code = @"

a := 1
b := 3
c := a + b
a := a + 1
print('c = ' + c)
print('a = ' + a)

arr  := ['hello','this','is','an','array']

print ('arr length is ' + (arr.'Length' + 100) )
print ('str length is ' + 'some string'.Length)

closureprint := x => print(x + a)
closureprint('foo')

each(element, arr)
{
    print(element)
}

for (a := 0; a < 10; a ++) 
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
}

(x => print('lambda fun ' + x))('yay')

if (true, print('hello'));

f := func(a,b,c) 
{
    print('abc '+a+' '+b+' '+c);
}

f(1)(2)(3);


Person := class (firstName,lastName)
{
    sayHello := func ()
    {
        print ('Hello ' + firstName + ' ' + lastName)
    }
}

john := Person('John','Doe');
jane := Person('Jane','Doe');

john.extra = Person('testing sub object','oh yes')

john.firstName = 'Johnny'

print ('john = ' + john.firstName + ' ' + john.lastName)
print ('jane = ' + jane.firstName + ' ' + jane.lastName)

john.sayHello();
jane.sayHello();
john.extra.sayHello();

list = LinkedList();
list.add('first');
list.add('second');
list.add('third');
list.add('last');
list.each(v => {
    print ('lamda ' + v);
});

s = Stack();
s.push(1);
s.push(2);
s.push(3);
s.push(4);

print (s.pop());
print (s.pop());
print (s.pop());
print (s.pop());
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