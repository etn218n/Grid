public interface IMover
{ 
    bool IsMoving { get; }
    void Move(Path path);
}