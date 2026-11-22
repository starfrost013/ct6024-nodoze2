// GOAAAL
// Each goal type uses the sensors in roder to achieve itself.

enum GoalState
{
    /// <summary>
    /// the pre-requisites fot this goal were not fulfilled
    /// </summary>
    NoPrerequisite = 0,
    Prerequisite = 1,

    /// <summary>
    /// this goal is being performed
    /// </summary>
    Active =2, 

    Completed = 3,
}

class Goal
{
    protected GoalState state; 

    virtual protected bool CheckPrerequisites()
    {
        return false;
    }

    virtual protected bool Perform()
    {
        return false;
    }
}
