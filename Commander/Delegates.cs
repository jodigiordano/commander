namespace EphemereGames.Commander
{
    using System.Collections.Generic;


    delegate void BooleanHandler(bool b);
    delegate void NoneHandler();
    delegate bool IntegerHandler(int i);
    delegate void Integer2Handler(int i);
    delegate void PanelWidgetHandler(PanelWidget p);
    delegate void NewsTypeHandler(NewsType t);
    delegate void NewsTypeNewsHandler(NewsType t, List<News> n);
    delegate void StringHandler(string s);
    delegate void DirectionHandler(Direction d);
}


namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;


    delegate void PhysicalObjectHandler(ICollidable o);
    delegate void PhysicalObjectPhysicalObjectHandler(ICollidable o1, ICollidable o2);
    delegate void SimPlayerHandler(SimPlayer p);
    delegate void SimPlayerDirectionHandler(SimPlayer p, Direction d);
    delegate void PausePlayerHandler(PausePlayer p);
    delegate void CommonStashHandler(CommonStash s);
    delegate void CelestialBodyHandler(CelestialBody c);
    delegate void NewGameStateHandler(GameState s);
    delegate void TurretHandler(Turret t);
    delegate void TurretSimPlayerHandler(Turret t, SimPlayer p);
    delegate void PowerUpTypeSimPlayerHandler(PowerUpType p, SimPlayer sp);
    delegate void PowerUpTypeHandler(PowerUpType p);
    delegate void PowerUpSimPlayerHandler(PowerUp p, SimPlayer pl);
    delegate void TurretTurretHandler(Turret t1, Turret t2);
    delegate void EnemyCelestialBodyHandler(Enemy e, CelestialBody c);
    delegate void BulletCelestialBodyHandler(Bullet b, CelestialBody c);
    delegate void TurretPhysicalObjectHandler(Turret t, ICollidable o);
    delegate void EnemyBulletHandler(Enemy e, Bullet b);
    delegate void EditorPlayerHandler(EditorPlayer e);
    delegate void EditorCommandHandler(EditorCommand e);
    delegate void NextWaveHandler(WaveDescriptor d);
    delegate void SimPlayerSimPlayerHandler(SimPlayer p1, SimPlayer p2);
    delegate void CollidableBulletHandler(ICollidable i, Bullet b);
    delegate void DestroyableHandler(IDestroyable i);
    delegate void BulletHandler(Bullet b);
}
