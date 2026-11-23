
import { useNavigate } from "react-router-dom";
import { ShieldX, ArrowLeft, LayoutDashboard } from "lucide-react";

function Unauthorized() {
    const navigate = useNavigate();

    return (
        <div className="flex min-h-[80vh] items-center justify-center bg-slate-50 px-4">
            <div className="w-full max-w-lg rounded-2xl border border-slate-200 bg-white p-8 text-center shadow-sm">

                {/* Icon */}
                <div className="mx-auto mb-6 flex h-20 w-20 items-center justify-center rounded-full bg-red-50">
                    <ShieldX size={42} className="text-red-600" />
                </div>

                {/* Error Code */}
                <h1 className="text-6xl font-bold tracking-tight text-slate-800">
                    403
                </h1>

                <h2 className="mt-3 text-xl font-semibold text-slate-800">
                    Access Denied
                </h2>

                <p className="mx-auto mt-3 max-w-sm text-sm leading-6 text-slate-500">
                    You don't have permission to access this page.
                    Please contact your administrator if you believe
                    this is a mistake.
                </p>

                {/* Actions */}
                <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">

                    <button
                        type="button"
                        onClick={() => navigate(-1)}
                        className="inline-flex items-center justify-center gap-2 rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50"
                    >
                        <ArrowLeft size={17} />
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
    );
}

export default Unauthorized;
