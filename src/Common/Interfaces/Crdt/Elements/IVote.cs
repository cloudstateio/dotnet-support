namespace CloudState.CSharpSupport.Interfaces.Crdt.Elements
{
  public interface IVote : ICrdt 
  {
    bool SelfVote { get; }  
    int Voters { get; }
    int VotesFor { get; }
    void Vote(bool vote);
    // bool IsAtLeastOne() => VotesFor > 0;
    // bool IsMajority() => VotesFor > Voters / 2;
    // bool IsUnanimous() => VotesFor == Voters;
  }
}
