using TheClimb.Core;
using UnityEngine;

namespace TheClimb.Astral
{
    public class PlanetCommandProvider    //  天体のコマンドプロバイダー
    {
     
        public OrbitalFollower orbitalFollower { get;}
        public RotationPlanet rotationPlanet{ get;}

        public PlanetCommandProvider(Transform PlanetTF, Transform PlayerTF, GravitationStatusBlock gravitationStat, OrbitalStatusBlock orbitalStat, OrbitalContext orbitalCtx)
        {
            orbitalFollower = new OrbitalFollower(orbitalCtx);
            rotationPlanet = new RotationPlanet(PlanetTF, gravitationStat);
        }
    }
}