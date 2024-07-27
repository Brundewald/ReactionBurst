namespace MyProject.ReactionBurst.SaveLoad
{
    public interface ISaveLoadService
    {
        public void Save();
        public void SetData(string path, object target);
        public T GetData<T>(string path);
        void ClearProgress();
    }
}