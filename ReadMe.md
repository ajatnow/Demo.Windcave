Creating a new PaymentProvider for an Umbraco Commerce site is not easy.
I needed to integrate the Windcave payment gateway into my site but there were no examples out there and the docs are not very helpful when one is new to this particular task.

The Umbraco docs are good as far as they go but I found it really didn't make a lot of sense until I had actually done it.

So this is my attempt at explaining how to create a new PaymentProvider for Umbraco Commerce using Windcave as an example.

Due to the initial confusion I encountered at the start of this project, mostly around what happens when and controlled by what, 
I have tried emphasize in this guide what functions are the responsibility of Umbraco Commerce, and what functions are the responsibility of the PaymentProvider.
There is also the point where neither are relevant and it is just the normal controller/view stuff.

>Please note that this is NOT a complete working solution. It is a guide to help you get started. 
>You will need to do some work to get it going for your particular situation.