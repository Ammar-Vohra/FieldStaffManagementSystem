function SummaryCard({ title, value, description, icon }) {
    return (
        <div className="rounded-xl border border-gray-200 bg-white p-5 shadow-sm">
            <div className="flex items-center justify-between gap-3">
                <div>
                    <p className="text-sm text-gray-500">{title}</p>

                    <h2 className="mt-2 text-3xl font-bold text-gray-900">
                        {value ?? 0}
                    </h2>

                    {description && (
                        <p className="mt-2 text-xs text-gray-400">
                            {description}
                        </p>
                    )}
                </div>

                <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-blue-50 text-2xl">
                    {icon}
                </div>
            </div>
        </div>
    );
}

export default SummaryCard;