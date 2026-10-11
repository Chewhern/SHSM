# Layman's explanations for programming language's mutable and immutable data types
Actually, we know or are aware that there are two main data types in programming languages: mutable and immutable.
I did make a video explaining these two types a while ago.
So, let me give a layman's overview.

## What is a mutable data type?
This represents writing and correcting data on a piece of paper with a pencil and eraser in real life.
In programming languages, this can be any array or pointer data type
(the latter doesn't represent the Java concept of an object, but rather something like ```unsigned char*, uint_8*, void**``` in C/C++).

## What is an immutable data type?
Imagine writing and correcting data on a piece of paper with a pen and correction tape in real life.
My question is...
Actually, when you use correction tape over something you've already written, you can observe that the old data itself isn't deleted.
You're actually creating a new space and writing on it.

This type of data is actually atomic data types ```int, char, float, double``` and similar data types to String, not including arrays.

## Continuing mutable and immutable data types
Every programming language provides both mutable and immutable data types by default.
Here, we must mention another concept: the garbage collector.

Imagine you live in a neighborhood or apartment building.
The frequency and timing of garbage collection services are inherently fixed and cannot be changed or accelerated.
You can think of it from the perspective of government personnel or the apartment building's garbage collectors;
these are essentially predetermined and cannot be changed or sped up.

**If sensitive/confidential data was written on a piece of paper using immutable data type manner in environment like actual physical office,
then that paper could be possibly passed to others and one don't really know whether 'other people' will print out copies.
This situation will continues until janitor comes and collect the papers that need to be recycled.
After janitor did came and collect the papers, one don't really know how long it will take for the papers to be transferred to proper recycling station from garbage stations.**

It's often said that modern programming languages ​​largely inherit from C/C++, but each language has its own design intent, so it's crucial to avoid generalizations.
If the confidential data you're handling comes from a String, and Strings are inherently immutable, then currently only C, C++, and C# are suitable languages ​​to attempt to clear the final String data.
I'm not very familiar with Rust/Zig.

You may or may not have encountered this type of confidential data handled by Strings,
but it's safe to say that this applies to PEM/Base64 certificate private keys from public key infrastructures,
web application API keys, and formally processed passwords.

This applies to companies like ```Ruijie, Alibaba, Baidu, and Tencent``` from **China** and ```Microsoft Azure, Amazon, Google``` from **US**.
However, this is actually a side-channel attack on the programming language itself.
To completely eliminate the data remaining in memory, process, and threads caused by using String or immutable data types... frankly, it's impossible.
The last accessible string data can be removed, but the specific number of times it was copied... these copies themselves cannot be eliminated.

Therefore, when developing functionalities for finance, cryptography, or network devices, try to limit yourself to C#, C++, or C.
This is easier said than done.

I'm not very familiar with C/C++, but in C, ```String``` can be replaced with ```unsigned char*, uint_8*, or void**``` when necessary and will still work normally. In C#, this is thanks to its **GCHandle** and **IntPtr**. You can look them up if needed.

# Reference
[Python Video](https://www.youtube.com/watch?v=0P-Dhb8kW5E)

From this video, one can generally do the same experiment with atomic data types like ```int,char,float,double``` and using data type of ```String``` which is also an object. For the stated data types, the layman explanation of how immutable data type works can be applied here.

If one does the experiment using mutable data types like arrays, the layman explanation of how mutable data type works can be applied here.

This underlying concepts can be applied to all programming languages.. but in the context of HSM or SHSM.., the programming language needs to have something like **C#'s** ```IntPtr, UIntPtr, GCHandle```. Lacking either one of these can't be used for HSM or SHSM core. 
