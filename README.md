# STUPID BRUTEFORCING OF THE SUBGLYPHS

## Intro
The numbers in "sequence" are just meant to be the subglyphs and not to point at a specific order[[1](https://imgur.com/a/mkxUwqm)]. The number of variation for 10 characters and 26 choices each is 19,275,223,968,000[[2](https://www.hackmath.net/en/calculator/combinations-and-permutations?n=26&k=10&order=1&repeat=0)] (and that's too much).

## Presumptions
As computations of all variations without repetition means factorial complexity, we needed to narrow down a few things:
1. We're going to assume that the subglyphs are roman letters we can "translate" such as "subglyph 1 = X, subglyph 2 = N" and so on.
2. We're assuming they are english words.
3. Given that the subglyph 2 is by far the most common and repeated 6 times, and looking at frequency analysis of the letters in english vocabulary[[3](https://www.lexico.com/explore/which-letters-are-used-most )], we're going to assume this is the "E" letter.
4. Our alphabet is going to be amputated of the 4 less commonly used letters in english vocabulary: Q, J, Z, X ( < 0.2% frequency)[[3](https://www.lexico.com/explore/which-letters-are-used-most )].
5. risky=iest asumption: we're going to assume that the subglyph 0, being between two "E"s, is a "Y". It's probably stupid, but we need to narrow those numbers down to something almost reasonable.

## Basic knowledge
With all those base criterias, we end up with 8 characters that can be any of 20 letters, which makes 5,079,110,400 possibilities.

I ran a test (in debug and for one minute, so give or take) locally, and I had on average 150,000 computed variations every minute. 


Based on that observation, computing and testing 5,079,110,400 possibilities may take around 23 days on a computer running the program 24/7.


## Language Detection 
Once the program computed a variation, we'll get it tested with a library that will determine the language that may be used in it[[4](https://github.com/ivanakcheurov/ntextcat)]. 
The issue is: the library (and many others, this one is actually performing pretty OK) is obviously struggling with the lack of space in the sentence.

But choices need to be made: I ran some tests, and the library gives a "level of confidence": 
- the lower that number is, the most confident the program is about its deduction, 
- trying 19 character long, non-spaced, english sentences gave me levels between 3953 and 3962,
- based on this, the program will only store english sentences with a level of confidence equal or smaller than 3964.


## Links & Resources
- [1] subglyphs and their designated number: https://imgur.com/a/mkxUwqm 
- [2] calculating the number of possibilities: https://www.hackmath.net/en/calculator/combinations-and-permutations?n=26&k=10&order=1&repeat=0
- [3] frequency of letters in english vocabulary: https://www.lexico.com/explore/which-letters-are-used-most 
- [4] NtextCat: https://github.com/ivanakcheurov/ntextcat
