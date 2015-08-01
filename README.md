# Plastic

A Functional and OOP experimental scripting language.
Conceptually inspired by Lisp and Ruby, syntactically by the C family.


### Functions

Standard function declaration:
```javascript
hello := func(who)
{
   print ('hello ' + who)
}
```

Or written as a lambda expression:
```javascript
hello := who => print ('hello ' + who)
```

Partial applications
```javascript
f := func(a,b,c)
{
    print('abc '+a+' '+b+' '+c);
}

f(1)(2)(3);

//or

//partial application of the "for" function
repeat10 = for(i := 0;i < 10;i++)

//apply the last argument, the body
repeat10
{
   print (i)
}

```


### Macros

Macros in plastic is not a separate concept, they are normal functions in which some of the arguments are passed as expressions rather than evaluated values.

```javascript
myMacro := func (someScalar, someExpression.ref)
{
     repeat(someScalar)
     {
        someExpression()
     }
}
```

### Expressions

In Plastic, everything is an expression.

### Assignment

* `:=` assigns a value to a name in the current lexical scope.
* `=` assigns/updates the value in the scope it was declared

Pattern matching

```javascript
(:dostuff,1,foo,bar) = (:dostuff,1,"hello","plastic")
```
`foo` is now "hello" and `bar` = "plastic"

### Classes

```javascript
Person := class (firstName,lastName)
{
    sayHello := func ()
    {
        print ('Hello {0} {1}',firstName,lastName)
    }
}

john := Person('John','Doe');
john.sayHello();
```

### Mixins

```
BeepMixin := mixin
{
    beep := func ()
    {
        print ('beep')
    }
}

Person := class (firstName,lastName)
{
    BeepMixin()
    sayHello := func ()
    {
        print ('Hello {0} {1}',firstName,lastName)
    }
}

john := Person('John','Doe');
john.beep();
```

### .NET Interop

```javascript
Console := using (System.Console);
Console.WriteLine('Name?');
name = Console.ReadLine();
Console.WriteLine('Hello {0}', name);
```
