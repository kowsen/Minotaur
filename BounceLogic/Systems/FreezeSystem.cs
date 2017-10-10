using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace BounceLogic
{
    public class FreezeSystem : EntitySystem
    {
        public FreezeSystem()
        {
            AddComponentRequirement<PositionComponent>();
            _r = new Random();
        }

        private double counter;
        private bool isFrozen = false;
        private Random _r;

        public override void Update(TimeSpan time, EntitySet entities)
        {
            counter += time.TotalMilliseconds;
            if (counter > 3000)
            {
                counter = 0;
                if (isFrozen)
                {
                    for (var i = 0; i < entities.Entities.Count; i++)
                    {
                        entities.Entities[i].AddComponent(new VelocityComponent(_r.Next(1, 5), _r.Next(1, 5)));
                    }
                }
                else
                {
                    var movingEntities = entities.Query(ComponentSignatureManager.GenerateComponentSignature(new List<Type>() { typeof(VelocityComponent) }, null));
                    for (var i = 0; i < movingEntities.Entities.Count; i++)
                    {
                        movingEntities.Entities[i].RemoveComponentOnNextTick(typeof(VelocityComponent));
                    }
                }
                isFrozen = !isFrozen;
            }
        }
    }
}
