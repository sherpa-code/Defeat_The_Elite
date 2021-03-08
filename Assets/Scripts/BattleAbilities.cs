using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilities : MonoBehaviour
{
    /**
     * Dictionary:
     * Name : {attributes}
     * 
     * Dictionary:
     * Attribute name : value
     */

    //public Dictionary<string, Dictionary<string, dynamic>> melee = new Dictionary<string, Dictionary<string, dynamic>>();
    //public Dictionary<string, Dictionary<string, dynamic>> melee = new Dictionary<string, Dictionary<string, dynamic>>()
    //{
    //    "Bite" : { "Damage" : 5 }
    //};


    //Dictionary<string, string> melee = new Dictionary<string, string>()
    //{
    //    { "Bite" , "1" },
    //    { "Swipe", "3" }
    //};

    //Dictionary<string, Dictionary<string, string>> melee2 = new Dictionary<string, Dictionary<string, string>>()
    //{
    //    { "Bite" , { "Attack", "1" },
    //    { "Swipe", { "Attack", "3" }
    //};


    //Dictionary<string, string> melee = new Dictionary<string, string>()
    //{
    //    { "Bite" , "1" }
    //};



    //Dictionary<string, Dictionary<string, string>> melee2 = new Dictionary<string, Dictionary<string, string>>()
    //{
    //    { "Bite" , "Attack" : "1" }
    //};



    /**
     * Melee Abilities (Melee Attack)
     */
    Dictionary<string, string> bite = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Bite" },
        { "damage", "3" }
    };

    Dictionary<string, string> whip = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Whip" },
        { "damage", "3" }
    };

    Dictionary<string, string> slap = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Slap" },
        { "damage", "3" }
    };

    Dictionary<string, string> smash = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Smash" },
        { "damage", "3" }
    };

    Dictionary<string, string> tailSwipe = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Tail Swipe" },
        { "damage", "3" }
    };

    Dictionary<string, string> splash = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Splash" },
        { "damage", "3" }
    };

    Dictionary<string, string> fang = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Fang" },
        { "damage", "3" }
    };

    Dictionary<string, string> prick = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Prick" },
        { "damage", "3" }
    };

    Dictionary<string, string> crunch = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Crunch" },
        { "damage", "3" }
    };

    Dictionary<string, string> playRough = new Dictionary<string, string>()
    {
        { "category", "melee" },
        { "name", "Play Rough" },
        { "damage", "3" }
    };


    /**
     * Special Abilities (Special Moves)
     */
    Dictionary<string, string> poisonStrike = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "Poison Strike" },
        { "damage", "3" }
    };

    Dictionary<string, string> leafCutter = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "LeafCutter" },
        { "damage", "3" }
    };

    Dictionary<string, string> regenerate = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "regenerate" },
        { "damage", "3" }
    };

    Dictionary<string, string> counter = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "Counter" },
        { "damage", "3" }
    };

    Dictionary<string, string> screech = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "Screech" },
        { "damage", "3" }
    };

    Dictionary<string, string> sludge = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "Sludge" },
        { "damage", "3" }
    };

    Dictionary<string, string> sleep = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "Sleep" },
        { "damage", "3" }
    };

    Dictionary<string, string> volt = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "Volt" },
        { "damage", "3" }
    };

    Dictionary<string, string> roar = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "Roar" },
        { "damage", "3" }
    };

    Dictionary<string, string> luckyStrike = new Dictionary<string, string>()
    {
        { "category", "special" },
        { "name", "Lucky Strike" },
        { "damage", "3" }
    };

}
