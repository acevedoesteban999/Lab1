## Modelado de Forms en C# de las clases Forms de [ESE_CRT](https://github.com/Esteban1914/ESE_CRT) y [ESE_GRS](https://github.com/Esteban1914/ESE_GRS) 

# Se ha modelado:
> - Forms			(Abstracta)
> - SelectionForm 		(Abstracta)(Hereda de Forms)
> - Button			(Hereda de Forms)
> - RadioButton			(Hereda de SelectionForm) 
> - ChectBox			(Hereda de SelectionForm)
> - Label			(Hereda de Forms)
> - Container			(Hereda de Forms)
> - Box				(Hereda de Container)
> - FreeBox			(Hereda de Container)
> - StackForm				

En el Main se ha implementado un sistema de busqueda de elementos pulsados para
un posterior uso en el evento OnClick, para ello escribir la coordenada X,
seguido de la corrdenada Y para una encontrar el elemento pulsado en el 
StackForms, el cual contiene todos los elementos.
 
## Estudiante
>***Esteban Acevedo Santana #1 A31***