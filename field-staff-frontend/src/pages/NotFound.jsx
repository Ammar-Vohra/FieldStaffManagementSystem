import { useNavigate } from "react-router-dom";
import { FileQuestion, ArrowLeft, LayoutDashboard } from "lucide-react";


function NotFound() {
    const navigate = useNavigate();

    return (
        <div className="flex min-h [80vh] items-center justify-center bg-slate 50 px-6 pt-35">
            <div className="w-full max-w-lg rounded-2xl border border-slate-200 bg-white p-8 text-center shadow-sm">
                
                <div className="mx-auto mb-6 flex h-20 w-20 items-center justify-center rounded-full bg-blue-50">
                    <FileQuestion size={42} className="text-blue-600" />
                </div>

                <h1 className="text-6xl font-bold tracking-tight text-slate-800">
                    404
                </h1>

                <h2 className="mt-3 text-xl font-semibold text-slate-800">
                    Page Not Found
                </h2>

                <p className="mx-auto mt-3 max-w-sm text-sm leading-6 text-slate-500">
                    Sorry, the page you're looking for doesn't exist
                    or may have been moved. Please check the URL
                    and try again.
                </p>

                <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">

                    <button 
                    type="button" 
                    onClick={() => navigate(-1)}
                    className="inline-flex items-center justify-center gap-2 rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50"
                    >
                        <ArrowLeft size={17}/>
                            Go Back
                    </button>


                   <button
                        type="button"
                        onClick={() => navigate("/dashboard", { replace: true })}
                        className="inline-flex items-center justify-center gap-2 rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-medium text-white transition hover:bg-blue-700"
                    >
                        <LayoutDashboard size={17} />
                        Dashboard
                    </button>
                </div>
            
            </div>


        </div>
    )
}

export default NotFound;