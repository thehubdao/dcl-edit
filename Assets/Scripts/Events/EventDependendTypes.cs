using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public static class EventDependentTypes
    {
        public struct UiBuilderSetupKey : IComparable<UiBuilderSetupKey>
        {
            public enum UiBuilderId
            {
                Hierarchy,
                Inspector,
                Settings
            }

            public UiBuilderId Id;

            public string SubId;

            public bool IsSubBuilder() => (SubId ?? "") != "";

            public static bool operator !=(UiBuilderSetupKey self, UiBuilderSetupKey other)
            {
                return !(self == other);
            }

            public static bool operator ==(UiBuilderSetupKey self, UiBuilderSetupKey other)
            {
                if (self.Id != other.Id)
                    return false;

                if ((self.SubId ?? "") != (other.SubId ?? ""))
                    return false;

                return true;
            }

            public int CompareTo(UiBuilderSetupKey other)
            {
                var idComparison = Id.CompareTo(other.Id);
                if (idComparison != 0) return idComparison;
                return string.Compare((SubId ?? ""), (other.SubId ?? ""), StringComparison.Ordinal);
            }

            public bool Equals(UiBuilderSetupKey other)
            {
                return Id == other.Id && (SubId ?? "") == (other.SubId ?? "");
            }

            public override bool Equals(object obj)
            {
                return obj is UiBuilderSetupKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return ((int) Id << 26) + (SubId ?? "").GetHashCode();
            }
        }
    }
}
