
function Pagination({
    pageNumber,
    pageSize,
    totalPages,
    totalRecords,
    hasPreviousPage,
    hasNextPage,
    onPageChange,
    onPageSizeChange,
    loading = false
}) {
    return (
        <div className="bg-white border border-slate-200 rounded-xl p-4 flex flex-col md:flex-row items-center justify-between gap-4">

            {/* Record Count and Page Size */}
            <div className="flex items-center gap-3">

                <p className="text-sm text-slate-600">
                    Total Records: <strong>{totalRecords}</strong>
                </p>

                <select
                    value={pageSize}
                    onChange={(e) => onPageSizeChange(Number(e.target.value))}
                    disabled={loading}
                    className="border border-slate-300 rounded-lg px-3 py-2 text-sm"
                >
                    <option value={5}>5 per page</option>
                    <option value={10}>10 per page</option>
                    <option value={20}>20 per page</option>
                    <option value={50}>50 per page</option>
                    <option value={100}>100 per page</option>
                </select>

            </div>

            {/* Navigation */}
            <div className="flex items-center gap-3">

                <button
                    onClick={() => onPageChange(pageNumber - 1)}
                    disabled={!hasPreviousPage || loading}
                    className="px-4 py-2 border rounded-lg text-sm disabled:opacity-40 disabled:cursor-not-allowed hover:bg-slate-100"
                >
                    Previous
                </button>

                <span className="text-sm text-slate-600 whitespace-nowrap">
                    Page <strong>{pageNumber}</strong> of <strong>{totalPages}</strong>
                </span>

                <button
                    onClick={() => onPageChange(pageNumber + 1)}
                    disabled={!hasNextPage || loading}
                    className="px-4 py-2 border rounded-lg text-sm disabled:opacity-40 disabled:cursor-not-allowed hover:bg-slate-100"
                >
                    Next
                </button>

            </div>

        </div>
    );
}

export default Pagination;
