namespace CheckBox.Model
{
    public class CategoryModel
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string FirstLetter
        {
            get { return Name[0].ToString().ToUpper(); } // Получаем первую букву и преобразуем её в верхний регистр
        }
    }
}