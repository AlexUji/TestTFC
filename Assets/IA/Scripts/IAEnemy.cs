using System;
using System.Collections;
using System.Collections.Generic;
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
                tilesInrange =  rangeFinder.GetTilesInRange(ts.IAInfo.selectedTroop.activeTile, ability.abilityRange);

                foreach (OverlayTile tile in tilesInrange)
                {
                    if (tile.characterInTile != null && tile.characterInTile.team == Team.Ally && ability.type == UnitType.Attacker)
                        ts.IAInfo.CharacterInRange_plus_ability.Add((tile.characterInTile,ability));

                    else if(tile.characterInTile != null && tile.characterInTile.team == Team.Enemy && ability.type == UnitType.Support)
                        ts.IAInfo.CharacterInRange_plus_ability.Add((tile.characterInTile, ability));
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
        /*
        List<(Troop, Troop)> attackPairs; // Lista de ataques posibles        

        public void SetValues(List<(Troop, Troop)> _attackPairs)
        {
            attackPairs = _attackPairs;

        }
        public override void Action()
        {
            GameManager.Instance.enemyTroops = GameManager.Instance.GetTroops(Team.Red); // Todas las tropas enemigas     

            // Lógica para seleccionar el mejor ataque
            (Troop, Troop) atacantes = SelectBestAttack(attackPairs);
            Troop attacker = atacantes.Item1;
            Troop target = atacantes.Item2;
            Debug.Log($"{attacker.name} ataca a {target.name}");

            // Realizamos el ataque
            if (attacker is Tower)
            {
                GameManager.Instance.board.AttackWithTroop(attacker, target);
            }
            else
            {
                attacker.Attack(target);
            }

            GameManager.Instance.UseAction();//ESTO CUANDO SE META EN EL ARBOL BIEN HAY QUE QUITARLO DE AQUI
            Debug.Log("Ataque realizado");
        }

        public (Troop attacker, Troop target) SelectBestAttack(List<(Troop attacker, Troop target)> attackPairs)
        {
            (Troop bestAttacker, Troop bestTarget) bestAttack = (null, null);
            float bestDamage = float.MinValue;

            foreach (var attackPair in attackPairs)
            {
                Troop attacker = attackPair.attacker;
                Troop target = attackPair.target;

                // Check if the attack will kill the target
                if (target.health - target.damage <= 0)
                {
                    // If this attack kills the target, choose it
                    return attackPair;
                }

                // Otherwise, check if this attack deals more damage than the previous best
                if (target.damage > bestDamage)
                {
                    bestDamage = target.damage;
                    bestAttack.bestAttacker = attacker;
                    bestAttack.bestTarget = target;
                }
            }

            return bestAttack;
        }*/
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
    {/*
        List<Troop> playerTroops;
        List<Troop> enemyTroops;

        Node[] path;
        int steps;

        Troop actualTroop;

        int index;

        public override void Action()
        {
            Debug.Log("Mover");
            playerTroops = GameManager.Instance.playerTroops;
            enemyTroops = GameManager.Instance.enemyTroops;

            path = null;
            steps = 1000;
            index = 0;

            actualTroop = null;

            if (path == null && playerTroops.Count > 0 && enemyTroops.Count > 0)
            {
                foreach (Troop ETroop in enemyTroops)
                {
                    foreach (Troop PTroop in playerTroops)
                    {
                        if (ETroop == null || PTroop == null) continue;

                        Cell ECell = ETroop.transform.parent.GetComponent<Cell>();
                        Cell PCell = PTroop.transform.parent.GetComponent<Cell>();

                        if (ECell == null || PCell == null) continue;

                        (int, int) EPos = ECell.GetGridPosition();
                        (int, int) PPos = PCell.GetGridPosition();

                        // Calcula el vector de dirección desde PPos hacia EPos
                        int dirX = EPos.Item1 - PPos.Item1;
                        int dirY = EPos.Item2 - PPos.Item2;

                        // Normaliza el vector (reduce a una dirección de paso único)
                        int stepX = dirX != 0 ? dirX / Math.Abs(dirX) : 0;
                        int stepY = dirY != 0 ? dirY / Math.Abs(dirY) : 0;

                        // Calcula la posición ajustada (por ejemplo, a 1 celda de distancia)
                        int adjustedX = PPos.Item1 + stepX * ETroop.attackRange; // Máximo 1 paso
                        int adjustedY = PPos.Item2 + stepY * ETroop.attackRange;

                        (int, int) adjustedPos = (adjustedX, adjustedY);

                        if (ETroop.moveRange < 1)
                            AllTowers(enemyTroops);

                        else
                            PathRequestManager.RequestPath(EPos, adjustedPos, OnPathFound, ETroop.moveRange);

                    }
                }
            }
            else if (path == null)
            {
                if (enemyTroops.Count == 0)
                {
                    Debug.LogError("No hay tropas enemigas disponibles.");
                    return;
                }

                index = 0;
                actualTroop = enemyTroops[index];
                while (index < enemyTroops.Count && actualTroop != null && actualTroop.moveRange < 1)
                {
                    index++;
                    if (index < enemyTroops.Count)
                    {
                        actualTroop = enemyTroops[index];
                    }
                }

                if (index >= enemyTroops.Count || actualTroop == null)
                {
                    AllTowers(enemyTroops);
                }
                else
                {
                    Cell ECell = actualTroop.transform.parent.GetComponent<Cell>();
                    if (ECell == null)
                    {
                        Debug.LogError("La celda de la tropa actual es nula.");
                        return;
                    }

                    (int, int) EPos = ECell.GetGridPosition();

                    float maxInfluence = 0;
                    (int, int) actualPos = (0, 0);

                    for (int i = 0; i < GameManager.Instance.board.rows; i++)
                    {
                        for (int j = 0; j < GameManager.Instance.board.columns; j++)
                        {
                            if (GameManager.Instance.board.GetCellInfluence(i, j) >= maxInfluence)
                                if (IsCloser(EPos, actualPos, (i, j)))
                                {
                                    actualPos = (i, j);
                                    maxInfluence = GameManager.Instance.board.GetCellInfluence(i, j);
                                }
                        }
                    }

                    PathRequestManager.RequestPath(EPos, actualPos, OnDiferentPath, actualTroop.moveRange);
                }
            }
        }

        public static bool IsCloser((int x, int y) point1, (int x, int y) point2, (int x, int y) point3)
        {
            float distance1 = MathF.Pow(point3.x - point1.x, 2) + MathF.Pow(point3.y - point1.y, 2);
            float distance2 = MathF.Pow(point3.x - point2.x, 2) + MathF.Pow(point3.y - point2.y, 2);

            return distance1 < distance2;
        }

        public void AllTowers(List<Troop> enemyTroops)
        {
            bool towers = true;
            foreach (Troop ETroop in enemyTroops)
                if (ETroop != null && ETroop.moveRange != 0)
                    towers = false;

            if (towers)
                GameManager.Instance.UseAction();
        }

        public void OnPathFound(Node[] newPath, bool pathSuccessful)
        {
            if (newPath == null)
            {
                Debug.LogError("El camino encontrado es nulo.");
                return;
            }

            int actual = index;
            index++;
            if (pathSuccessful)
            {
                if (path == null || steps > newPath.Length)
                {
                    path = newPath;
                    steps = path.Length;
                    actualTroop = enemyTroops[Math.Min(actual / playerTroops.Count, enemyTroops.Count - 1)];
                }
            }
            if (path != null && path.Length > 0 && index >= playerTroops.Count * enemyTroops.Count)
            {
                Cell destination = GameManager.Instance.board.GetCell(path[0].gridY, path[0].gridX);

                GameManager.Instance.board.MoveTroop(actualTroop, destination);
            }
        }

        public void OnDiferentPath(Node[] newPath, bool pathSuccessful)
        {
            if (newPath == null || newPath.Length == 0)
            {
                Debug.LogError("El camino diferente es inválido.");
                return;
            }

            Cell destination = GameManager.Instance.board.GetCell(newPath[0].gridY, newPath[0].gridX);

            GameManager.Instance.board.MoveTroop(actualTroop, destination);
        }*/
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