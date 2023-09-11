using Application1.Models;

namespace Application1.ViewModels
{
    public abstract class DeletableViewModel<ThisT, ModelT> : ViewModel<ThisT, ModelT> where ThisT : DeletableViewModel<ThisT, ModelT> where ModelT : Model<ModelT>
    {
        // Construction

        protected DeletableViewModel(ModelT model) : base(model)
        {
        }

        // Destruction

        void __Delete()
            => _all.Remove((ThisT)this);

        // Static construction

        static DeletableViewModel()
        {
            // Когда модель удалена, удалим представляющую её модель представления при наличии.
            Model<ModelT>.OnDeleted += (model) =>
            {
                foreach (var one in _all)
                    if (one.Model == model)
                    {
                        one.__Delete();
                        break;
                    }
            };
        }
    }
}
