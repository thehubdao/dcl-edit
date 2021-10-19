using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EntityComponent: MonoBehaviour
{
    public struct Ts
    {
        public Ts(string symbol, string setup)
        {
            this.symbol = symbol;
            this.setup = setup;
        }

        public string symbol;
        public string setup;
    }

    public abstract Ts GetTypeScript(string entityName);
    //{
    //    return new Ts( $"{entityName}setup", $"const {entityName}setup = new BoxShape()\n");
    //}
}
