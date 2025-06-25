using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.UI.Image;

public struct IAInfo
{
    public GameObject enemyTeam;
    public GameObject allyTeam;
    public CharacterInfo selectedTroop;
    public List<CharacterInfo> enemiesInRange;
    public List<(CharacterInfo,Ability)> CharacterInRange_plus_ability;
    public List<OverlayTile> posibleBestTilesForMovement;
    public float amountOfInfluence;
    public int indexPath;
    public List<OverlayTile> finalPath;
    public bool isMoving;
}

public class IAEnemy : MonoBehaviour
{

    IANode n_root;
    TurnSistem turnSistem;
    private float thinkTimer = 0; // Temporizador
    private float velocity = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        turnSistem = TurnSistem.Instance;
        InitializeIA();
        Think();
    }

    private void Update()
    {
        if (turnSistem.EnemyTurn)
        {

            if (turnSistem.IAInfo.isMoving)
            {
                velocity = 0.001f;
            }
            else{
                velocity = 0.5f;
            }
            thinkTimer += Time.deltaTime;
            
            if (thinkTimer >= velocity)
            {
                Think(); // Llamar a la función Think
                thinkTimer = 0f; // Reiniciar el temporizador
            }
        }
        else
        {
            thinkTimer = 0f;
        }
    }

    void Think()
    {
        if (turnSistem.EnemyTurn)
        {
            n_root.Action();
        }
    }


    private void InitializeIA()
    {
        

        // iniciamos nodos hoja (acciones)
       
        IABasicAttack basic_attack = new IABasicAttack();
        IAHability ability = new IAHability();
        IAMove move = new IAMove();
        IASkipTurn skip = new IASkipTurn();



        // atamos nodos a los padres (sequence nodes) 

        IABasicAttackRange hasUnitsInBasicAttackRange = new IABasicAttackRange(basic_attack,skip);
        IAHasMove hasMoved = new IAHasMove(hasUnitsInBasicAttackRange,move);
        IAHabilityRange hasUnitsInHabilityRange = new IAHabilityRange(ability, hasMoved);
        IAHasAttack hasAttacked = new IAHasAttack(move,hasUnitsInHabilityRange);
        IAHasAction hasAction = new IAHasAction(hasAttacked,skip);

        n_root = hasAction;

        // Seleccionamos a la primera unidad a evaluar

        //turnSistem.InitialiceIAInfo();
        //turnSistem.SelectTroop();
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------
    // -- NODOS DECISIONES
    // -----------------------------------------------------------------------------------------------------------------------------------------

    class IAHasAction : IASequenceNode
    {
        public IAHasAction(IANode nodeTrue, IANode nodeFalse) : base(nodeTrue, nodeFalse) { }

        public override void Action()
        {
            
            if (TurnSistem.Instance.IAInfo.selectedTroop.haveActions)
            {
                n_true.Action();
            }
            else
            {
                n_false.Action();
            }
        }
    }

    class IAHasMove : IASequenceNode
    {
        public IAHasMove(IANode nodeTrue, IANode nodeFalse) : base(nodeTrue, nodeFalse) { }

        public override void Action()
        {
            if (TurnSistem.Instance.IAInfo.selectedTroop.haveMoved)
            {
                n_true.Action();
            }
            else
            {
                n_false.Action();
            }
        }
    }

    class IAHasAttack : IASequenceNode
    {
        public IAHasAttack(IANode nodeTrue, IANode nodeFalse) : base(nodeTrue, nodeFalse) { }

        public override void Action()
        {
            if (TurnSistem.Instance.IAInfo.selectedTroop.haveAttacked)
            {
                n_true.Action();
            }
            else
            {
                n_false.Action();
                
            }
        }
    }

    class IAHabilityRange : IASequenceNode
    {
        public IAHabilityRange(IANode nodeTrue, IANode nodeFalse) : base(nodeTrue, nodeFalse) { }

        public override void Action()
        {
            RangeFinder rangeFinder = new RangeFinder();
            TurnSistem ts = TurnSistem.Instance;
            List<OverlayTile> tilesInrange;
            ts.IAInfo.CharacterInRange_plus_ability.Clear();


            foreach (Ability ability in ts.IAInfo.selectedTroop.habilities)
            {
                if (ability.manaCost <= ts.IAInfo.selectedTroop.currentMP && ability.levelToUnlock <= ts.IAInfo.selectedTroop.level) //Puedes usar la habilidad
                {
                    tilesInrange = rangeFinder.GetTilesInRange(ts.IAInfo.selectedTroop.activeTile, ability.abilityRange); //Obtienes el rango de acción de la habilidad

                    foreach (OverlayTile tile in tilesInrange)
                    {
                        //Compruebas si se puede usar en función del tipo de habilidad que se esta usando
                        if (tile.characterInTile != null && tile.characterInTile.team == Team.Ally && ability.type == UnitType.Attacker)
                            ts.IAInfo.CharacterInRange_plus_ability.Add((tile.characterInTile, ability));

                        else if (tile.characterInTile != null && tile.characterInTile.team == Team.Enemy && ability.type == UnitType.Support)
                            ts.IAInfo.CharacterInRange_plus_ability.Add((tile.characterInTile, ability));
                    }
                }

            }


            if (ts.IAInfo.CharacterInRange_plus_ability.Count > 0)
            {

                n_true.Action();
            }
            else
            {
                //Debug.Log("No está a rango de habilidades");
                n_false.Action();
            }
        }

    }

    class IABasicAttackRange : IASequenceNode
    {
        public IABasicAttackRange(IANode nodeTrue, IANode nodeFalse) : base(nodeTrue, nodeFalse) { }

        public override void Action()
        {
            RangeFinder rangeFinder = new RangeFinder();
            TurnSistem ts = TurnSistem.Instance;
            List<OverlayTile> tilesInrange = rangeFinder.GetTilesInRange(ts.IAInfo.selectedTroop.activeTile, ts.IAInfo.selectedTroop.atackRange);
            ts.IAInfo.enemiesInRange.Clear();

            foreach (OverlayTile tile in tilesInrange)
            {
                if (tile.characterInTile != null && tile.characterInTile.team == Team.Ally)
                    ts.IAInfo.enemiesInRange.Add(tile.characterInTile);
            }

            if(ts.IAInfo.enemiesInRange.Count > 0)
            {
                Debug.Log("Si esta a rango de basic");
                n_true.Action();
            }
            else
            {
                Debug.Log("No esta a rango de basic");
                n_false.Action();
                
            }
        }

    }

    // -----------------------------------------------------------------------------------------------------------------------------------------
    // -- NODOS HOJA
    // -----------------------------------------------------------------------------------------------------------------------------------------
    class IABasicAttack : IANode
    {
        TurnSistem ts = TurnSistem.Instance;
        List<CharacterInfo> enemiesInRange = new List<CharacterInfo> ();
        RangeFinder rangeFinder = new RangeFinder();
        CharacterInfo slectedEnemy = null;



        public override void Action()
        {
            enemiesInRange = ts.IAInfo.enemiesInRange;
            ts.UpdateAmountOfInfluence();
            float previousAmountOfInfluence = ts.IAInfo.amountOfInfluence;
            float simulatedAmountofInfluence = previousAmountOfInfluence;

            foreach (CharacterInfo enemy in enemiesInRange)
            {
                int enemyLife = enemy.currentHP;
                enemyLife -= ts.IAInfo.selectedTroop.attack - enemy.defense;
                
                //Mataria a la tropa enemiga
                if (enemyLife <= 0)
                {
                    simulatedAmountofInfluence -= 2;

                    List<OverlayTile> neighboursTiles = MapManager.Instance.GetNeighboursNodes(enemy.activeTile, rangeFinder.GetTilesInRange(enemy.activeTile, 1));
                    foreach (OverlayTile nTile in neighboursTiles)
                    {
                        simulatedAmountofInfluence -= 0.25f;
                    }

                }
                //No la mataria
                else
                {
                    simulatedAmountofInfluence -= 0.25f;
                }
                
                //Comprueba si es la mejor jugada hasta el momento
                if(simulatedAmountofInfluence < previousAmountOfInfluence)
                {
                    previousAmountOfInfluence = simulatedAmountofInfluence;
                    slectedEnemy = enemy;
                }
                   

                simulatedAmountofInfluence = ts.IAInfo.amountOfInfluence;
            }

            //Realiza la mejor acción y actualiza la influencia si es necesario
            ts.IAInfo.selectedTroop.IAbasicAttack(slectedEnemy);
            Debug.Log("Ataque basico");

        }
    }

    class IAHability : IANode
    {
        //CharacterInRange_plus_ability
        TurnSistem ts = TurnSistem.Instance;
        List<(CharacterInfo, Ability)> enemiesInRange = new List<(CharacterInfo, Ability)> ();
        (CharacterInfo, Ability) finalAction = (null,null);
        CharacterInfo character;

        public override void Action()
        {
            enemiesInRange = ts.IAInfo.CharacterInRange_plus_ability;
            ts.UpdateAmountOfInfluence();
            character = ts.IAInfo.selectedTroop;

            float previousAmountOfInfluence = ts.IAInfo.amountOfInfluence;
            float simulatedAmountofInfluence = previousAmountOfInfluence;
            float score = 0;
           // int manaUsed = Int32.MaxValue;

            foreach ((CharacterInfo, Ability) action in enemiesInRange)
            {
                
                score = SimulateAbility(action.Item2, character, action.Item1);
                Debug.Log("Score "+score);
                simulatedAmountofInfluence -= score;

                if(simulatedAmountofInfluence < previousAmountOfInfluence)
                {
                    previousAmountOfInfluence = simulatedAmountofInfluence;
                    finalAction = action;
                }
            }

           
            finalAction.Item2.ApplyEffect(character,finalAction.Item1);
            Debug.Log(finalAction.Item2.abilityInfoText);
        }

        public int SimulateAbility(Ability ability, CharacterInfo self, CharacterInfo target)
        {
            int score = 0;
            if (self.type == UnitType.Attacker) //Si es de tipo atacante se prioriza atacar a enemigos
            {
                Debug.Log("Atacante");
                switch (ability.type)
                {
                    case UnitType.Attacker:
                        
                        int life = ability.SimulateAttack(self, target);
                        life = target.currentHP - life;
                        Debug.Log("Vida quitada" + life);
                        if (life <= 0)
                            score = 100;
                        else
                            score = 50;
                        break;

                    case UnitType.Debuff:
                        score = ability.SimulateDebuff(self, target);
                        break;
                    case UnitType.Support:
                        int lifeH = ability.SimulateHeal(self, target);
                        if ((lifeH * 100) / target.MaxHP >= 75)
                        {
                            score = 20;
                        }
                        else
                        {
                            score = 10;
                        }
                        break;
                    case UnitType.Buff:
                        score = ability.SimulateBuff(self, target);
                        break;

                }
            }
            else // Si es de tipo soporte se prioriza ayudar a aliados
            {
                switch (ability.type)
                {
                    case UnitType.Attacker:
                        int life = ability.SimulateAttack(self, target);
                        life = target.currentHP - life;
                        if (life <= 0)
                            score = 100;
                        else
                            score = 50;
                        break;

                    case UnitType.Debuff:
                        score = ability.SimulateDebuff(self, target);
                        break;
                    case UnitType.Support:
                        int lifeH = ability.SimulateHeal(self, target);
                        if ((lifeH * 100) / target.MaxHP >= 75)
                        {
                            score = 150;
                        }
                        else
                        {
                            score = 90;
                        }
                        break;
                    case UnitType.Buff:
                        score = ability.SimulateBuff(self, target);
                        break;

                }
            }

            return score;
        }
    }

    class IAMove : IANode
    {
        TurnSistem ts = TurnSistem.Instance; 
        RangeFinder rangeFinder = new RangeFinder();
        List<OverlayTile> bestMovementTiles;
        List<OverlayTile> movementRange;
        List<OverlayTile> path;
        OverlayTile bestTile = null;
        OverlayTile finalTile = null;
        PathFinder pathFinder = new PathFinder();

        float tiempoDeEspera = 3f;
        float tiempoTranscurrido = 0f;
        bool accionEjecutada = false;


        public override void Action()
        {
            if (!ts.IAInfo.isMoving)
            {
                Debug.Log("No se ha movido");
                movementRange = rangeFinder.GetTilesInRange(ts.IAInfo.selectedTroop.activeTile, ts.IAInfo.selectedTroop.movementRange);
                bestMovementTiles = ts.IAInfo.posibleBestTilesForMovement; //Obtenemos las tile vacias que tienen influencia
                float bestScore = 0;


                //Primero obtenemos la mejor tile a la que se podría ir en función del tipo de unidad
                if (ts.IAInfo.selectedTroop.type == UnitType.Attacker) // Si es de tipo atacante nos aproximamos a los enemigos
                {
                    foreach (OverlayTile tile in bestMovementTiles)
                    {
                        int multy = 1;
                        if (!tile.isBlocked)
                        {
                            List<OverlayTile> range1 = rangeFinder.GetTilesInRange(tile, 1);
                            foreach (OverlayTile tileInFront in range1)
                            {
                                if (tileInFront.characterInTile != null && tileInFront.characterInTile.team == Team.Ally)
                                    multy += 100;

                            }

                            if (!tile.isBlocked && movementRange.Contains(tile))
                            {
                                multy += 5;
                            }
                            if (!tile.isBlocked && (tile.influence * multy) > bestScore)
                            {
                                bestScore = tile.influence * multy;
                                bestTile = tile;
                                ts.BestTile = tile;
                            }

                        }
                    }
                }
                else //Si es de tipo soporte nos acercamos a nuestros aliadados
                {
                    foreach (OverlayTile tile in bestMovementTiles)
                    {
                        int multy = 1;
                        if (!tile.isBlocked && movementRange.Contains(tile))
                        {
                            multy = 5;
                        }
                        if (!tile.isBlocked && (tile.influence * multy) < bestScore)
                        {
                            bestScore = tile.influence * multy;
                            bestTile = tile;
                            ts.BestTile = tile;
                        }
                    }
                }



                // Comprobamos si esta dentro del rango de movimiento de nuestra unidad

                path = pathFinder.FindPath(ts.IAInfo.selectedTroop.activeTile, bestTile, movementRange);

                if (path.Contains(bestTile)) //Si esta en el rango nos movemos directamente a esta
                {
                    Debug.Log("Mejor tile dentro del path");

                    bestTile.characterInTile = ts.IAInfo.selectedTroop;
                    finalTile = bestTile;
                }
                else //Si no esta obtenemos el tile dentro de nuestro rango que este más cerca
                {
                    int distance = Int32.MaxValue;
                    foreach (OverlayTile tile in movementRange)
                    {
                        if (!tile.isBlocked)
                        {
                            path = pathFinder.FindPath(tile, bestTile, ts.MapinTiles);
                            if (path.Count < distance) //Está a una distancia menor de la tile deseada
                            {
                                distance = path.Count;
                                finalTile = tile;
                                ts.BestTile = tile;
                            }
                        }
                    }
                    //Nos guardamos el path con la tile más cernaca a la deseada
                    path = pathFinder.FindPath(ts.IAInfo.selectedTroop.activeTile, finalTile, movementRange);
                    finalTile.characterInTile = ts.IAInfo.selectedTroop;
                }

                ts.IAInfo.selectedTroop.activeTile.characterInTile = null;
                ts.IAInfo.selectedTroop.activeTile.isBlocked = false;

                Debug.Log("PATH" + path.Count);
                //Hacemos el path hasta el final
                ts.IAInfo.finalPath = path;
                ts.IAInfo.isMoving = true;
            }
            else
            {
                Debug.Log("Se está moviendo");
                path = ts.IAInfo.finalPath;
                Debug.Log("FINAL PATH" + path.Count);

                if (path.Count <= 0)
                {
                    ts.IAInfo.selectedTroop.haveMoved = true;
                }
                else
                {

                    if (path.Count > 0)
                    {
                        List<OverlayTile> miniPath = new List<OverlayTile>();

                        miniPath.Add(path[0]);

                        if (path.Count >= 2)
                        {
                            miniPath.Add(path[1]);

                        }


                        MoveCharacterAlongPath(miniPath, ts.IAInfo.indexPath);
                           
                        

                    }
                }
                ts.IAInfo.finalPath = path;
            }
           
          

        }


        public void PositionOnTile(OverlayTile tile )
        {
            
            ts.IAInfo.selectedTroop.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            ts.IAInfo.selectedTroop.activeTile = tile;
        }
        private void MoveCharacterAlongPath(List<OverlayTile> minipath, int index)
        {
            Debug.Log("Minipath");
            var steep = 10 * Time.deltaTime;
            CharacterInfo character = ts.IAInfo.selectedTroop;

            var zIndex = minipath[0].transform.position.z;
            character.transform.position = Vector2.MoveTowards(character.transform.position, minipath[0].transform.position, steep);
            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

            if (Vector2.Distance(character.transform.position, minipath[0].transform.position) < 0.0001f)
            {
                PositionOnTile(minipath[0]);
                path.RemoveAt(index);
                
            }

            if (path.Count == 0)
            {
                finalTile.isBlocked = true;
                character.haveMoved = true;
                ts.IAInfo.isMoving = false;
               

            }

        }

        
    }

    class IASkipTurn : IANode
    {
        public override void Action()
        {
            CharacterInfo character = TurnSistem.Instance.IAInfo.selectedTroop;
            character.haveAttacked = true;
            character.haveMoved = true;
            TurnSistem.Instance.EndAction(character);
            TurnSistem.Instance.UpdateInfluence();
        }
    }

    
}