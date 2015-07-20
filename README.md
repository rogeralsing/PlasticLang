# Plastic

A Functional and OOP experimental scripting language.
Conceptually inspired by Lisp and Ruby, syntactically by the C family.


### Functions




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

`:=` assigns a value to a name in the current lexical scope.
`=` assigns/updates the value in the scope it was declared

### Classes

### Mixins

