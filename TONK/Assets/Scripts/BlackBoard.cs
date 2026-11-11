using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  CLASS BLACKBOARD
 *  This is the main blackboard that tanks will use as a centralized point of knowledge
 *  Tanks will be able to write and read from this class
 *  Here is what the blackboard will store:
 *  -   The blackboard will store all team information, like position, health, etc.
 *  -   The blackboard will store information on enemies that have been spotted by team members
 *  -   The blackboard will store all active bids that teammates have created
 *  As of right now, the squad leader will contain the blackboard
 *  At game start, every teammate will store a reference to their team's blackboard
 */
public class BlackBoard
{
    public List<TankState> tankStates; // stores lists giving attributes of each team on team
    public List<List<object>> enemySightings; // stores enemies that the "team" can see
    public List<Auction> auctions; // stores auctions that tanks have called


}

/*  CLASS AUCTION
 *  This will be the basis for all types of requests made my tanks 
 *  When a tank needs help or requires the utility of another tank, it will create an auction
 *  Here is what auctions can do:
 *  -   An auction can be to request backup by any other tank
 *  -   An auction can be to mark an enemy as a high priority target    ** This will likely only be made by the squad leader**
 *  -   An auction can []
 *  Once the auction has ended, it will pick the best bid that was made and assign the bidder for the task
 */
public class Auction
{
    private int auctionId;
    private int requesterId; // this is the ID of the tank who is calling

    private List<Bid> bids; // all of the active bids for this auction

    public Bid GetWinner() // picks the winner of the auction
    {
        Bid bid = null;
        return bid;
    }
}

public class Bid
{
    private TankState bidder;

    private void PlaceBid(int auctionId, TankState bidder)
    {
        // Places a bid at auction auctionId
    }
}

/*
 * CLASS TANKSTATE
 * This class wraps attributes of each tank into an object to make it easier to organize
 */
public class TankState
{
    private int id;
    private Vector2 position;
    private int health;

    private ROLE role;

    private float threatLevel; // How in danger a tank is
}

public enum ROLE
{
    ATTACK,
    DEFENSE,
    NORMAL,
    RANGER,
    PATROL
}
