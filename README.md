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

### Assignment

* `:=` assigns a value to a name in the current lexical scope.
* `=` assigns/updates the value in the scope it was declared

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
