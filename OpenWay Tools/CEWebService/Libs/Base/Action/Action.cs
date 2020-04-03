using System.Linq;
using System.Text;

namespace Itron.Ami.CEWebServiceClient.Base
{
    public interface ICEAction
    {
        //Executes a request that does not support a callback to the client. 
        void Execute();

        //Unique identifier for which to match against the callback.
        string UniqueID
        { get; }

        //Cancels the action (job) created by the request.
        void Cancel();
    }
}
