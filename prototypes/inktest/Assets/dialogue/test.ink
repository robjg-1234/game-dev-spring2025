VAR test = 6

-> Start
=== Start ===
This is the start of the story

* What is a cow? ->END
* Hey hello how are you.
    Perhaps.
    {test <5:
        Take me home.
        -else: country roads.
    }
-> END
