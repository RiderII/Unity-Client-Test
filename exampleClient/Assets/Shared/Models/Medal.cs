using System;

public class Medal
{
    private Challenge challengeType;
    private string sprite;
    private DateTime obtained;
    private string description;

    public Medal() { }

    public Medal(Challenge challengeType, string sprite, DateTime obtained, string description) {
        this.challengeType = challengeType;
        this.sprite = sprite;
        this.obtained = obtained;
        this.description = description;
    }
}

