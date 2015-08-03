using System;
using System.Linq;
using Akka.Actor;
using PlasticLang.Ast;

namespace PlasticLang.Akka
{
    public static class AkkaIntegration
    {
        public static PlasticActorSystem CreateSystem(string name)
        {
            return new PlasticActorSystem(name);
        }
    }

    public class PlasticActorSystem
    {
        private readonly ActorSystem _system;

        public PlasticActorSystem(string name)
        {
            _system = ActorSystem.Create(name);
        }

        public PlasticMacro actorOf
        {
            get
            {
                PlasticMacro m = (ctx, args) =>
                {
                    var c = new PlasticContextImpl(ctx.Parent);
                    
                    var actorState = new PlasticObject(c);
                    var body = args.First();
                    Action<object> a = msg =>
                    {
                        c.Declare("message",msg);
                        body.Eval(c);
                    };
                    var actor = _system.ActorOf(Props.Create(() => new PlasticActor(a)));
                    return actor;
                };
                return m;
            }
        }

        private void Nop()
        {
            
        }
    }

    public class PlasticActor : UntypedActor
    {
        private readonly Action<object> _receive;
        public PlasticActor(Action<object> receive)
        {
            _receive = receive;
        }

        protected override void OnReceive(object message)
        {
            _receive(message);
        }
    }
}
