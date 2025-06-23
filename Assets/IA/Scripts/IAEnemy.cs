using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor.Experimental.GraphView;
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
    public CharacterInfo selectedEnemyTroop;
    public List<CharacterInfo> enemiesInRange;
    public List<CharacterInfo> alliesInRange;
    public List<(CharacterInfo,Ability)> CharacterInRange_plus_ability;
    public List<OverlayTile> posibleBestTilesForMovement;
    public float amountOfInfluence;
}

public class IAEnemy : MonoBehaviour
{

    IANode n_root;
    TurnSistem turnSistem;
    private float thinkTimer = 0f; // Temporizador

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        turnSistem = TurnSistem.Instance;
        InitializeIA();
        Think();
    }

    private void Update()
    {
        /*if (!gm.yourTurn)
        {
            thinkTimer += Time.deltaTime;

            if (thinkTimer >= 1.5f)
            {
                Think(); // Llamar a la función Think
                thinkTimer = 0f; // Reiniciar el temporizador
            }
        }
        else
        {
            thinkTimer = 0f;
        }*/
    }

    void Think()
    {
        /*if (!gm.yourTurn)
        {
            n_root.Action();
        }*/
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

        turnSistem.InitialiceIAInfo();
        turnSistem.SelectTroop();
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

            foreach (OverlayTile tile in tilesInrange)
            {
                if (tile.characterInTile != null && tile.characterInTile.team == Team.Ally)
                    ts.IAInfo.enemiesInRange.Add(tile.characterInTile);
            }

            if(ts.IAInfo.enemiesInRange.Count > 0)
            {
                n_true.Action();
            }
            else
            {
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
            
        }
    }

    class IAHability : IANode
    {
        /*
       private List<(int, int)> ObtenerPosicionesValidas(Troop tropa, BoardGrid tablero)
       {
           List<(int, int)> posibleDeploypos = new List<(int, int)>();

           //Si es una barril
           if (tropa is Bomb)
           {
               for (int i = 0; i < tablero.rows; i++)
               {
                   for (int j = 0; j < tablero.columns; j++)
                   {
                       if (tablero.GetCell(i, j).GetColorTeam() == Team.Red || tablero.GetCell(i, j).GetColorTeam() == Team.None)
                       {
                           if (tablero.GetCell(i, j).transform.childCount == 0)
                           {
                               posibleDeploypos.Add((i, j));
                           }
                       }

                   }
               }
           }
           //El resto
           else
           {
               for (int i = 0; i < tablero.rows; i++)
               {
                   for (int j = 0; j < tablero.columns; j++)
                   {
                       if (tablero.GetCell(i, j).GetColorTeam() == Team.Red && tablero.GetCell(i, j).transform.childCount == 0)
                           posibleDeploypos.Add((i, j));
                   }
               }
           }
           return posibleDeploypos;
       }

       private ((float, Troop), (int, int)) ObtenerPosicionPorInfluencia(Troop tropa, List<(int, int)> posicionesParaComp, BoardGrid tablero)
       {
           float influence = 0f;
           float bestInfluence = -1f;
           (int, int) bestPos = (-1, -1);

           //Caballero
           if (tropa == GameManager.Instance.enemyTroopPrefabs[0])
           {
               //Recorremos todas las pos validas
               foreach ((int, int) pos in posicionesParaComp)
               {
                   //Comprobar si esta a mele
                   for (int x = -1; x < 2; x++)
                   {
                       for (int y = -1; y < 2; y++)
                       {
                           if (pos.Item1 + x >= 0 && pos.Item1 + x < tablero.rows && pos.Item2 + y >= 0 && pos.Item2 + y < tablero.columns)
                           {
                               if (x == 0 && y == 0) continue;

                               float t = tablero.GetCellInfluence(pos.Item1 + x, pos.Item2 + y);
                               if (t >= 0) influence += t;
                           }
                       }
                   }

                   influence /= 8f;

                   if (influence > bestInfluence)
                   {
                       bestInfluence = influence;
                       bestPos = pos;
                   }
               }

               Debug.Log("Caballero: " + bestInfluence);

           }

           //Arquero
           if (tropa == GameManager.Instance.enemyTroopPrefabs[1])
           {
               //Recorremos todas las pos validas
               foreach ((int, int) pos in posicionesParaComp)
               {
                   int closeEnemies = 0;
                   //Comprobar si puede atacar enemigos
                   for (int x = -2; x < 3; x++)
                   {
                       for (int y = -2; y < 3; y++)
                       {
                           if (pos.Item1 + x >= 0 && pos.Item1 + x < tablero.rows && pos.Item2 + y >= 0 && pos.Item2 + y < tablero.columns)
                           {
                               if (x <= 1 && x >= -1 && y <= 1 && y >= -1)
                               {
                                   if (tablero.GetCell(pos.Item1 + x, pos.Item2 + y).transform.childCount > 0)
                                   {
                                       closeEnemies++;
                                   }
                                   continue;
                               }

                               float t = tablero.GetCellInfluence(pos.Item1 + x, pos.Item2 + y);
                               if (t >= 0) influence += t;
                           }
                       }

                       influence = (influence - closeEnemies) / 12f;

                       if (influence > bestInfluence)
                       {
                           bestInfluence = influence;
                           bestPos = pos;
                       }
                   }
               }
               Debug.Log("Arquero: " + bestInfluence);
           }
           //Torre
           //Solo si tenemos 2 o mas tropas
           //Se tiene que colocar en zonas seguras
           if (tropa is Tower && GameManager.Instance.enemyTroops.Count >= 2)
           {
               //Recorremos todas las pos validas
               foreach ((int, int) pos in posicionesParaComp)
               {
                   //Comprobar si puede pintar o si tiene enemigos cerca
                   for (int x = -2; x < 3; x++)
                   {
                       for (int y = -2; y < 3; y++)
                       {
                           if (pos.Item1 + x >= 0 && pos.Item1 + x < tablero.rows && pos.Item2 + y >= 0 && pos.Item2 + y < tablero.columns)
                           {
                               if (x == 0 && y == 0) continue;

                               float t = tablero.GetCellInfluence(pos.Item1 + x, pos.Item2 + y);
                               influence += t;
                           }
                       }
                   }

                   influence /= -16f;

                   if (influence > bestInfluence)
                   {
                       bestInfluence = influence;
                       bestPos = pos;
                   }
               }
               Debug.Log("Torre: " + bestInfluence);
           }

           //Pawn solo si vamos mejor en casillas
           if (tropa == GameManager.Instance.enemyTroopPrefabs[3] && GameManager.Instance.enemyTroops.Count >= 2)
           {

               //Recorremos todas las pos validas
               foreach ((int, int) pos in posicionesParaComp)
               {
                   int closeEnemies = 0;
                   //Comprobar si puede atacar enemigos
                   for (int x = -3; x < 4; x++)
                   {
                       for (int y = -3; y < 4; y++)
                       {
                           if (pos.Item1 + x >= 0 && pos.Item1 + x < tablero.rows && pos.Item2 + y >= 0 && pos.Item2 + y < tablero.columns)
                           {
                               if (x <= 1 && x >= -1 && y <= 1 && y >= -1)
                               {
                                   if (tablero.GetCell(pos.Item1 + x, pos.Item2 + y).transform.childCount > 0)
                                   {
                                       closeEnemies++;
                                   }
                                   continue;
                               }

                               float t = tablero.GetCellInfluence(pos.Item1 + x, pos.Item2 + y);
                               if (t >= 0) influence += t;
                           }
                       }

                       influence = (influence - closeEnemies) / 16f;

                       if (influence > bestInfluence)
                       {
                           bestInfluence = influence;
                           bestPos = pos;
                       }
                   }
               }
               Debug.Log("Mago: " + bestInfluence);
           }

           //Barrel
           //Tiene que usarse matando a la mayor cantidad de tropas posibles
           //Hay que desplegarlo donde se tenga más influencia enemiga
           //Solo lo usaremos si tenemos alguna tropa
           if (tropa is Bomb && GameManager.Instance.enemyTroops.Count >= 1)
           {

               //Recorremos todas las pos validas
               foreach ((int, int) pos in posicionesParaComp)
               {
                   //Comprobar si esta a mele
                   for (int x = -1; x < 2; x++)
                   {
                       for (int y = -1; y < 2; y++)
                       {
                           if (pos.Item1 + x >= 0 && pos.Item1 + x < tablero.rows && pos.Item2 + y >= 0 && pos.Item2 + y < tablero.columns)
                           {
                               if (x == 0 && y == 0) continue;

                               if (tablero.GetCell(pos.Item1 + x, pos.Item2 + y).transform.childCount > 0)
                                   influence += 2.5f;
                               if (tablero.GetCell(pos.Item1 + x, pos.Item2 + y).GetColorTeam() == Team.Blue)
                                   influence += 1f;
                               else if (tablero.GetCell(pos.Item1 + x, pos.Item2 + y).GetColorTeam() == Team.None)
                                   influence += 0.5f;
                           }
                       }
                   }

                   influence /= 8f;

                   if (influence > bestInfluence)
                   {
                       bestInfluence = influence;
                       bestPos = pos;
                   }
               }
               Debug.Log("Bomba: " + bestInfluence);
           }
           return ((bestInfluence, tropa), bestPos);
       }

       public (Troop, (int, int)) ObtenerMejorJugada()
       {
           Troop tropaSeleccionada = null;

           List<(int, int)> posibleDeploypos = new List<(int, int)>();
           (int, int) deployPos = (0, 0);
           List<((float, Troop), (int, int))> List_score_pos = new List<((float, Troop), (int, int))>();
           float previousBestPlay = 0;


           for (int i = 0; i < GameManager.Instance.enemyTroopPrefabs.Count; i++)
           {
               //Si tenemos suficiente dinero para comprar la tropa simulamos la mejor jugada
               if (GameManager.Instance.GetCoins(Team.Red) >= GameManager.Instance.enemyTroopPrefabs[i].cost)
               {
                   //Funciona
                   posibleDeploypos = ObtenerPosicionesValidas(GameManager.Instance.enemyTroopPrefabs[i], GameManager.Instance.board);

                   ((float, Troop), (int, int)) bestDeploypos = ObtenerPosicionPorInfluencia(GameManager.Instance.enemyTroopPrefabs[i], posibleDeploypos, GameManager.Instance.board);

                   //Guardamos la jugada
                   List_score_pos.Add(bestDeploypos);
               }

           }

           previousBestPlay = List_score_pos[0].Item1.Item1;
           tropaSeleccionada = List_score_pos[0].Item1.Item2;
           deployPos = List_score_pos[0].Item2;
           foreach (((float, Troop), (int, int)) jugada in List_score_pos)
           {
               //Debug.Log("Tropa en el array "+jugada.Item1.Item2);
               if (jugada.Item1.Item1 > previousBestPlay)
               {
                   Debug.Log("Tropa seleccionada " + jugada.Item1.Item2);
                   previousBestPlay = jugada.Item1.Item1;
                   tropaSeleccionada = jugada.Item1.Item2;
                   deployPos = jugada.Item2;
               }
           }

           // Si previousBestPlay sigue siendo 0, selecciona una tropa y posición aleatorias
           if (previousBestPlay == 0)
           {
               // Filtrar tropas disponibles según el coste
               List<Troop> tropasDisponibles = GameManager.Instance.enemyTroopPrefabs
                   .Where(t => GameManager.Instance.GetCoins(Team.Red) >= t.cost)
                   .ToList();

               if (tropasDisponibles.Count > 0)
               {
                   // Seleccionar tropa aleatoria
                   tropaSeleccionada = tropasDisponibles[UnityEngine.Random.Range(0, tropasDisponibles.Count)];

                   // Obtener posiciones válidas para esa tropa
                   posibleDeploypos = ObtenerPosicionesValidas(tropaSeleccionada, GameManager.Instance.board);

                   // Seleccionar posición aleatoria
                   if (posibleDeploypos.Count > 0)
                   {
                       deployPos = posibleDeploypos[UnityEngine.Random.Range(0, posibleDeploypos.Count)];
                   }
               }
           }

           return (tropaSeleccionada, deployPos);
       }

       public override void Action()
       {
           Debug.Log("Desplegar");

           (Troop, (int, int)) jugada = ObtenerMejorJugada();

           if (jugada.Item2.Item1 > -1)
           {
               GameManager.Instance.board.SpawnTroop(jugada.Item1, GameManager.Instance.board.GetCell(jugada.Item2.Item1, jugada.Item2.Item2));
               GameManager.Instance.SpendCoins(jugada.Item1.cost, Team.Red);
           }
       }
       */
    }

    class IAMove : IANode
    {
        TurnSistem ts = TurnSistem.Instance; 
        RangeFinder rangeFinder = new RangeFinder();
        CharacterInfo slectedEnemy = null;
        List<OverlayTile> bestMovementTiles;
        List<OverlayTile> movementRange;
        List<OverlayTile> path;
        OverlayTile bestTile = null;
        OverlayTile finalTile = null;
        PathFinder pathFinder = new PathFinder();

        public override void Action()
        {
            bestMovementTiles = ts.IAInfo.posibleBestTilesForMovement; //Obtenemos las tile vacias que tienen influencia
            float bestScore = 0;
            

            //Primero obtenemos la mejor tile a la que se podría ir
            if(ts.IAInfo.selectedTroop.type == UnitType.Attacker)
            {
                foreach (OverlayTile tile in bestMovementTiles)
                {
                    if (tile.influence > bestScore)
                    {
                        bestScore = tile.influence;
                        bestTile = tile;
                    }
                }
            }
            else
            {
                foreach (OverlayTile tile in bestMovementTiles)
                {
                    if (tile.influence < bestScore)
                    {
                        bestScore = tile.influence;
                        bestTile = tile;
                    }
                }
            }

            // Comprobamos si esta dentro del rango de movimiento de nuestra unidad
            movementRange = rangeFinder.GetTilesInRange(ts.IAInfo.selectedTroop.activeTile, ts.IAInfo.selectedTroop.movementRange);
            
            if (movementRange.Contains(bestTile)) //Si esta en el rango nos movemos directamente a esta
            {
                path = pathFinder.FindPath(ts.IAInfo.selectedTroop.activeTile, bestTile, movementRange);
            }
            else //Si no esta obtenemos el tile dentro de nuestro rango que este más cerca
            {
                int distance = Int32.MaxValue;
                foreach (OverlayTile tile in movementRange)
                {
                    path = pathFinder.FindPath(tile, bestTile, ts.MapinTiles);
                    if (path.Count < distance) //Está a una distancia menor de la tile deseada
                    {
                        distance = path.Count;
                        finalTile = tile;
                    }
                }
                //Hacemos movimiento a la tile más cercana
                path = pathFinder.FindPath(ts.IAInfo.selectedTroop.activeTile, finalTile, movementRange);

            }

            while (path.Count > 0)
            {
                MoveCharacterAlongPath();
            }

        }


        public void PositionOnTile(OverlayTile tile)
        {
            Debug.Log("Pos x: " + tile.transform.position.x + ", Pos y: " + tile.transform.position.y + ", Pos z: " + tile.transform.position.z);
            ts.IAInfo.selectedTroop.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            //character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            ts.IAInfo.selectedTroop.activeTile = tile;
        }
        private void MoveCharacterAlongPath()
        {

            var steep = 5 * Time.deltaTime;
            CharacterInfo character = ts.IAInfo.selectedTroop;

            var zIndex = path[0].transform.position.z;
            character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, steep);
            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

            if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
            {
                PositionOnTile(path[0]);
                path.RemoveAt(0);
            }

            if (path.Count == 0)
            {
                character.haveMoved = true;
                ts.UpdateInfluence();

            }

        }
    }

    class IASkipTurn : IANode
    {
        public override void Action()
        {
            /* Debug.Log("Pasar turno");
             GameManager.Instance.SkipTurn();*/
        }
    }


}