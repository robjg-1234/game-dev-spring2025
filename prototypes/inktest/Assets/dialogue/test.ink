VAR firstTime = true
VAR known = false
VAR money = 0
VAR ringHaver = false
VAR openedDoor = false
VAR aware = false

=== Start ===
A few weeks had passed since the disappearance of the Red Diamond.
It seemed that every lead was a dead end.
Recently, relevant information reached your department, and you were tasked to investigate an old parking lot.
-> DONE

=== Tom ===
{known == false:
    {firstTime == false:
        {aware ==true:
            Changed your mind?
                    * Yes.
                    If you are looking for information, I can offer you some help for a price.
                        ** How much are we talking?
                            How much can you offer?
                            ->Offer
                        ** Not a chance.
                            ->DONE
                    * No.
                        ->DONE
        -else:
            Are you not satisfied with my answer?
                * You definitely know more than what you are willing to admit.
                    Well, my information has a price, and it is not cheap.
                            ** How much are we talking?
                                How much can you offer?
                                ~ aware = true
                                ->Offer
                            ** I'll keep looking.
                                Your loss.
                                ~ aware = true
                                ->DONE
                * I am.
                    Well, leave me alone.
                    ->DONE
        }
    -else:
        Who are you, and what are you doing in a place such as this?
            * Working.
                Hmm...
                ->DONE
            * Hello, my name is Clint Darren, a detective at the Oris Police Department, and I'm currently investigating the heist of the Red Diamond.
                Detective, huh? What leads you to believe that I can provide any help?
                    ** Nothing but certain information has led us to believe that important information can be found around this place.
                        Well, you are out of luck; there is nothing of value here.
                        ~ firstTime = false
                        ->DONE
                    ** There isn't anything that leads me to believe otherwise.
                        Well, my information has a price, and it is not cheap.
                        *** How much are we talking?
                            How much can you offer?
                            ~ aware = true
                            ~ firstTime = false
                            ->Offer
                        *** We don't negotiate with terrorists.
                            Your loss.
                            ~ aware = true
                            ~ firstTime = false
                            -> DONE
    }
-else:
    That's all I know.
    ->DONE
}

== Offer ==
*{money > 4}[I can offer 5 gold coins]
    padding
    A fair price for some information.
    From what I've heard, a group known as the Initiative of the Shining Star has been establishing underground stations around the city.
    They are all connected to one station, which is rumored to be below this place. I can't neither confirm nor deny it since I've never seen it myself, but you never know.
    That said, you never met me.
    ~ known = true
    ~money = money - 5
    ->DONE
* I'll think about it.
    Ok.
    ->DONE

=== smuggler ===
Need something?
* {known == true and ringHaver == false}[Do you know anything about the Initiative of the Shining Star?]
    padding
    I have this ring that I "borrowed" from someone who walked by here. You can take it for a price.
        ** {money > 4}[Sure I'll take it.]
            padding
            Pleasure doing business with you.
            ~ ringHaver = true
            ~money = money - 5
            ->DONE
        ** I'll think about it.
            I'll wait for you.
            ->DONE
* I'm looking for someone.
    Well, keep looking.
    ->DONE
    
=== guard ===
...
* {ringHaver == true}[(Show ring.)]
    padding
    ~openedDoor = true
    Go ahead.
    ->DONE
* Hello, my name is Clint Darren, a detective at the Oris Police Department, and I'm currently investigating the heist of the Red Diamond.
    ...
    ->DONE
    
=== order ===
Vive la Résistance!
->DONE
    



