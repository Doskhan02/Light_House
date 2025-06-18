using UnityEngine;

public class BossShipAIHandler : IAIComponent
{
    private Character character;
    private AllyBossCharacter bossCharacter;
    
    private Vector3[] waypoints;
    private int currentWaypointIndex = 0;
    private readonly float waypointDistanceThreshold = 2f;

    public void Initialize(Character selfCharacter)
    {
        character = selfCharacter;
        if (selfCharacter is AllyBossCharacter enemyBossCharacter)
        {
            bossCharacter = enemyBossCharacter;
            waypoints = bossCharacter.Waypoints;
            currentWaypointIndex = 0; // Start from first waypoint
        }
    }

    public void AIAction(Character target, AIState currentState, CharacterTypeData data)
    {
        if (data is BasicAllyData _data)
        {
            switch (currentState)
            {
                case AIState.MoveToTarget:
                    var direction = target.transform.position - character.transform.position;
                    character.movementComponent.Rotate(direction);
                    if (target is EnemyCharacter && Vector3.Distance(target.transform.position, character.transform.position) < 20)
                        return;
                    character.movementComponent.Move(direction);
                    break;

                case AIState.Attack:
                    // Implement attack logic here
                    break;

                case AIState.Idle:
                    FollowWaypoints();
                    break;
            }
        }
    }

    private void FollowWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Vector3 currentWaypoint = waypoints[currentWaypointIndex];
        Vector3 directionToWaypoint = currentWaypoint - character.transform.position;

        // Rotate towards waypoint
        character.movementComponent.Rotate(directionToWaypoint);

        // Move towards waypoint
        character.movementComponent.Move(directionToWaypoint);

        // If close enough to waypoint, move to next one
        if (directionToWaypoint.magnitude < waypointDistanceThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}