using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Declination
{
    string nominative, genitive, dative, accusative, ablative, locative;
    /// <summary>
    /// Constructor for decliantion class.
    /// </summary>
    /// <param name="nominative">Kto? Co?</param>
    /// <param name="genitive">Kogo? Czego?</param>
    /// <param name="dative">Komu? Czemu?</param>
    /// <param name="accusative">Kogo? Co?</param>
    /// <param name="ablative">Z kim? Z czym?</param>
    /// <param name="locative">O kim? O czym?</param>
    public Declination(string nominative, string genitive, string dative, string accusative, string ablative, string locative)
    {
        this.nominative = nominative;
        this.genitive = genitive;
        this.dative = dative;
        this.accusative = accusative;
        this.ablative = ablative;
        this.locative = locative;
    }
    /// <summary>
    /// Kto? Co?
    /// </summary>
    /// <returns></returns>
    public string getNominative()
    {
        return nominative;
    }
    /// <summary>
    /// Kogo? Czego?
    /// </summary>
    /// <returns></returns>
    public string getGenitive()
    {
        return genitive;
    }
    /// <summary>
    /// Komu? Czemu?
    /// </summary>
    /// <returns></returns>
    public string getDative()
    {
        return dative;
    }
    /// <summary>
    /// Kogo? Co?
    /// </summary>
    /// <returns></returns>
    public string getAccusative()
    {
        return accusative;
    }
    /// <summary>
    /// Z kim? Z czym?
    /// </summary>
    /// <returns></returns>
    public string getAblative()
    {
        return ablative;
    }
    /// <summary>
    /// O kim? O czym?
    /// </summary>
    /// <returns></returns>
    public string getLocative()
    {
        return locative;
    }
}
