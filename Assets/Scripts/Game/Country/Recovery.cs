using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;

public class Recovery : Stat
{
    private readonly Country _country;

    public const int RECOVERY_START = 14;

    public Recovery(Country country) {
        _country = country;
    }
}
