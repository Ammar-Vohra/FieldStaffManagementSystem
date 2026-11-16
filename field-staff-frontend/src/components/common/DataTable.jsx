
function DataTable({
  columns,
  data,
  loading = false,
  emptyMessage = "No records found.",
  getRowKey,
}) {
  // Loading State
  if (loading) {
    return (
      <div className="bg-white rounded-xl border border-slate-200 p-8 text-center">
        <p className="text-slate-500">Loading records...</p>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl border border-slate-200 overflow-hidden">

      <div className="overflow-x-auto">

        <table className="w-full text-sm text-left">

          {/* Table Header */}
          <thead className="bg-slate-50 text-slate-600 uppercase text-xs">
            <tr>
              {columns.map((column) => (
                <th
                  key={column.key}
                  className="px-6 py-4 font-semibold whitespace-nowrap"
                >
                  {column.label}
                </th>
              ))}
            </tr>
          </thead>

          {/* Table Body */}
          <tbody className="divide-y divide-slate-200">

            {data.length === 0 ? (
              <tr>
                <td
                  colSpan={columns.length}
                  className="px-6 py-10 text-center text-slate-500"
                >
                  {emptyMessage}
                </td>
              </tr>
            ) : (
              data.map((row, rowIndex) => (
                <tr
                  key={getRowKey ? getRowKey(row) : rowIndex}
                  className="hover:bg-slate-50 transition"
                >
                  {columns.map((column) => (
                    <td
                      key={column.key}
                      className="px-6 py-4 text-slate-700 whitespace-nowrap"
                    >
                      {column.render
                        ? column.render(row)
                        : row[column.key]}
                    </td>
                  ))}
                </tr>
              ))
            )}

          </tbody>

        </table>

      </div>

    </div>
  );
}

export default DataTable;
