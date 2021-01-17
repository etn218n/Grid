using System;
using MayBe;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using GridSystem;
using Random = UnityEngine.Random;

public class Pointer : MonoBehaviour
{
    [SerializeField] private GridEngine engine;

    public List<Character> selectedCharacters = new List<Character>();
    
    private void Start()
    {
        foreach (var character in selectedCharacters)
        {
            var validPosition = Maybe.None<Vector3>();

            while (!validPosition.HasValue)
            {
                var randomCoordinate = new Vector2Int(Random.Range(0, 20), Random.Range(0, 20));
                
                validPosition = engine.MovementGrid.GetTileAt(randomCoordinate)
                                                     .Filter(tile => tile.IsWalkable)
                                                     .Map(t => t.Position);
            }
            
            validPosition.MatchSome(point => character.transform.position = new Vector3(point.x, point.y, character.transform.position.z));
        }
    }

    private void TryMoveRandomly<T>(Character character, Grid<T> grid) where T : BaseTile<T>, IHaveMovementCost
    {
        var (source, destination) = RandomWaypointFor(grid, character);
            
        source.MatchSome(s => destination.MatchSome(d => character.Move(s, d)));
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            SetPathToCursorPosition();
        }
        
        // selectedCharacters.ForEach(character =>
        // {
        //     if (!character.Mover.IsMoving)
        //         TryMoveRandomly(character, engine.MovementGrid);
        // });
    }

    public void SetPathToCursorPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 10;
            
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        if (CanMoveTo(mouseWorldPosition, engine.MovementGrid))
            selectedCharacters.ForEach(character => Move(character, mouseWorldPosition));
    }

    public bool CanMoveTo(Vector3 destination, Grid<MovementTile> grid)
    {
        return grid.GetTileAt(destination).Filter(tile => tile.IsWalkable).HasValue;
    }

    public void Move(Character character, Vector3 destination)
    {
        var destinationTile = engine.MovementGrid.GetTileAt(destination);
        var characterTile   = engine.MovementGrid.GetTileAt(character.transform.position);

        LiftAction<MovementTile, MovementTile>((src, dest) => character.Move(src, dest))(characterTile, destinationTile);
    }

    public Action<Maybe<A>, Maybe<B>> LiftAction<A, B>(Action<A, B> action)
    {
        return (optionA, optionB) => optionA.MatchSome(a => optionB.MatchSome(b => action(a, b)));
    }

    public (Maybe<T>, Maybe<T>) RandomWaypointFor<T>(Grid<T> grid, Character character) where T : BaseTile<T>, IHaveMovementCost
    {
        var source = grid.GetTileAt(character.transform.position);
        var destination = Maybe.None<T>();

        while (!destination.HasValue)
        {
            var randomCoordinate = new Vector2Int(Random.Range(0, 50), Random.Range(0, 50));

            destination = grid.GetTileAt(randomCoordinate).Filter(tile => tile.IsWalkable);
        }

        return (source, destination);
    }
}
