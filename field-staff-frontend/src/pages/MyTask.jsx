
import { useEffect, useState } from "react";

import serviceTaskService from "../services/serviceTaskService";
import taskStatusService from "../services/taskStatusService";

import DataTable from "../components/common/DataTable";
import Modal from "../components/common/Modal";
import StatusBadge from "../components/common/StatusBadge";
import Pagination from "../components/data-display/Pagination";

function MyTasks() {
  const [tasks, setTasks] = useState([]);
  const [statuses, setStatuses] = useState([]);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [statusError, setStatusError] = useState("");

  // Pagination and search
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState("");

  const [totalRecords, setTotalRecords] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  // Task details
  const [selectedTask, setSelectedTask] = useState(null);
  const [detailsLoading, setDetailsLoading] = useState(false);
  const [detailsError, setDetailsError] = useState("");
  const [showDetailsModal, setShowDetailsModal] = useState(false);

  // Status update
  const [showUpdateModal, setShowUpdateModal] = useState(false);
  const [newStatusId, setNewStatusId] = useState("");
  const [remarks, setRemarks] = useState("");
  const [completionNote, setCompletionNote] = useState("");

  const [updateError, setUpdateError] = useState("");

  // Load assigned tasks
  const fetchMyTasks = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await serviceTaskService.getMyTasks({
        pageNumber,
        pageSize,
        search,
        sortBy: "scheduledDate",
        sortDirection: "desc",
      });

      setTasks(response.data || []);
      setTotalRecords(response.totalRecords || 0);
      setTotalPages(response.totalPages || 0);
    } catch (err) {
      setError(
        err.response?.data?.message ||
        "Unable to load your assigned tasks."
      );
    } finally {
      setLoading(false);
    }
  };

  // Load available task statuses
  const fetchStatuses = async () => {
    try {
      const response = await taskStatusService.getAllTaskStatuses();
      setStatuses(response || []);
    } catch (err) {
      setStatusError(
        err.response?.data?.message ||
        "Unable to load task statuses."
      );
    }
  };

  useEffect(() => {
    fetchMyTasks();
  }, [pageNumber, pageSize, search]);

  useEffect(() => {
    fetchStatuses();
  }, []);

  // Search
  const handleSearch = (e) => {
    setSearch(e.target.value);
    setPageNumber(1);
  };

  // Pagination
  const handlePageChange = (newPage) => {
    setPageNumber(newPage);
  };

  const handlePageSizeChange = (newSize) => {
    setPageSize(newSize);
    setPageNumber(1);
  };

  // Open task details
  const handleViewDetails = async (taskId) => {
    try {
      setDetailsLoading(true);
      setDetailsError("");
      setSelectedTask(null);
      setShowDetailsModal(true);

      const response =
        await serviceTaskService.getMyTaskDetails(taskId);

      setSelectedTask(response);
    } catch (err) {
      setDetailsError(
        err.response?.data?.message ||
        "Unable to load task details."
      );
    } finally {
      setDetailsLoading(false);
    }
  };

  // Open status update modal
  const handleOpenUpdateModal = () => {
    if (!selectedTask) return;

    setNewStatusId(String(selectedTask.taskStatusId));
    setRemarks("");
    setCompletionNote(selectedTask.completionNote || "");
    setUpdateError("");
    setShowUpdateModal(true);
  };

  // Update task status
  const handleUpdateStatus = async (e) => {
    e.preventDefault();

    if (!newStatusId) {
      setUpdateError("Please select a task status.");
      return;
    }

    try {
      setSaving(true);
      setUpdateError("");

      const response = await serviceTaskService.updateMyTaskStatus(
        selectedTask.serviceTaskId,
        {
          newStatusId: Number(newStatusId),
          remarks: remarks.trim() || null,
          completionNote: completionNote.trim() || null,
        }
      );

      setSelectedTask(response);
      setShowUpdateModal(false);

      // Refresh task list
      await fetchMyTasks();
    } catch (err) {
      setUpdateError(
        err.response?.data?.message ||
        "Unable to update task status."
      );
    } finally {
      setSaving(false);
    }
  };

  const formatDate = (date) => {
    if (!date) return "-";

    return new Date(date).toLocaleDateString("en-PK", {
      day: "2-digit",
      month: "short",
      year: "numeric",
    });
  };

  const formatDateTime = (date) => {
    if (!date) return "-";

    return new Date(date).toLocaleString("en-PK", {
      day: "2-digit",
      month: "short",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  // Custom badge because shared StatusBadge only supports IsActive
  const renderTaskStatus = (status) => {
    const statusName = status || "Unknown";

    const normalizedStatus = statusName.toLowerCase();

    let styles = "bg-slate-100 text-slate-700";

    if (
      normalizedStatus.includes("complete") ||
      normalizedStatus.includes("closed")
    ) {
      styles = "bg-green-100 text-green-700";
    } else if (
      normalizedStatus.includes("progress") ||
      normalizedStatus.includes("started")
    ) {
      styles = "bg-blue-100 text-blue-700";
    } else if (
      normalizedStatus.includes("pending") ||
      normalizedStatus.includes("assigned")
    ) {
      styles = "bg-amber-100 text-amber-700";
    } else if (normalizedStatus.includes("cancel")) {
      styles = "bg-red-100 text-red-700";
    }

    return (
      <span
        className={`inline-flex px-3 py-1 rounded-full text-xs font-medium ${styles}`}
      >
        {statusName}
      </span>
    );
  };

  // Main table columns
  const columns = [
    {
      key: "title",
      label: "Task",
    },
    {
      key: "customerName",
      label: "Customer",
    },
    {
      key: "scheduledDate",
      label: "Scheduled Date",
      render: (row) => formatDate(row.scheduledDate),
    },
    {
      key: "taskPriorityName",
      label: "Priority",
    },
    {
      key: "taskStatusName",
      label: "Status",
      render: (row) => renderTaskStatus(row.taskStatusName),
    },
    {
      key: "actions",
      label: "Actions",
      render: (row) => (
        <button
          type="button"
          onClick={() => handleViewDetails(row.serviceTaskId)}
          className="text-blue-600 hover:text-blue-800 font-medium"
        >
          View Details
        </button>
      ),
    },
  ];

  return (
    <div className="space-y-6">

      {/* Page Header */}
      <div>
        <h1 className="text-2xl font-bold text-slate-800">
          My Tasks
        </h1>

        <p className="text-sm text-slate-500 mt-1">
          View your assigned tasks and update their progress.
        </p>
      </div>

      {/* Search */}
      <div className="bg-white border border-slate-200 rounded-xl p-4">
        <input
          type="text"
          value={search}
          onChange={handleSearch}
          placeholder="Search tasks..."
          className="w-full md:w-80 border border-slate-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      {/* Error */}
      {error && (
        <div className="bg-red-50 text-red-700 border border-red-200 rounded-lg p-3 text-sm">
          {error}
        </div>
      )}

      {/* Tasks Table */}
      <DataTable
        columns={columns}
        data={tasks}
        loading={loading}
        emptyMessage="No assigned tasks found."
        getRowKey={(row) => row.serviceTaskId}
      />

      {/* Pagination */}
      <Pagination
        pageNumber={pageNumber}
        pageSize={pageSize}
        totalPages={totalPages}
        totalRecords={totalRecords}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        loading={loading}
      />

      {/* Task Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => setShowDetailsModal(false)}
        title="Task Details"
        size="2xl"
      >
        {detailsLoading ? (
          <p className="text-center text-slate-500 py-8">
            Loading task details...
          </p>
        ) : detailsError ? (
          <p className="text-red-600 text-sm">
            {detailsError}
          </p>
        ) : selectedTask ? (
          <div className="space-y-8 max-h-[78vh] overflow-y-auto pr-3">

            {/* Basic Information */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-6">

              <div>
                <p className="text-xs text-slate-500">Task Title</p>
                <p className="max-w-xs truncate font-medium text-slate-800">
                  {selectedTask.title}
                </p>
              </div>

              <div>
                <p className="text-xs text-slate-500">Customer</p>
                <p className="font-medium text-slate-800">
                  {selectedTask.customerName}
                </p>
              </div>

              <div>
                <p className="text-xs text-slate-500">Assigned Staff</p>
                <p className="font-medium text-slate-800">
                  {selectedTask.assignedStaffName}
                </p>
              </div>

              <div>
                <p className="text-xs text-slate-500">Scheduled Date</p>
                <p className="font-medium text-slate-800">
                  {formatDate(selectedTask.scheduledDate)}
                </p>
              </div>

              <div>
                <p className="text-xs text-slate-500">Priority</p>
                <p className="font-medium text-slate-800">
                  {selectedTask.taskPriorityName}
                </p>
              </div>

              <div>
                <p className="text-xs text-slate-500">Current Status</p>
                {renderTaskStatus(selectedTask.taskStatusName)}
              </div>

              <div>
                <p className="text-xs text-slate-500">Active</p>
                <StatusBadge isActive={selectedTask.isActive} />
              </div>

              <div>
                <p className="text-xs text-slate-500">Created At</p>
                <p className="font-medium text-slate-800">
                  {formatDateTime(selectedTask.createdAt)}
                </p>
              </div>

            </div>

            {/* Description */}
            <div>
              <h3 className="font-semibold text-slate-800 mb-2">
                Description
              </h3>

              <p className="text-sm text-slate-600 whitespace-pre-wrap">
                {selectedTask.description || "No description provided."}
              </p>
            </div>

            {/* Completion Note */}
            <div>
              <h3 className="font-semibold text-slate-800 mb-2">
                Completion Note
              </h3>

              <p className="text-sm text-slate-600 whitespace-pre-wrap">
                {selectedTask.completionNote || "No completion note added."}
              </p>
            </div>

            {/* Status History */}
            <div>
              <h3 className="font-semibold text-slate-800 mb-3">
                Status History
              </h3>

              <DataTable
                columns={[
                  {
                    key: "oldStatusName",
                    label: "Previous Status",
                    render: (row) => row.oldStatusName || "Initial",
                  },
                  {
                    key: "newStatusName",
                    label: "New Status",
                    render: (row) => renderTaskStatus(row.newStatusName),
                  },
                  {
                    key: "userName",
                    label: "Updated By",
                  },
                  {
                    key: "remarks",
                    label: "Remarks",
                    render: (row) => row.remarks || "-",
                  },
                  {
                    key: "createdAt",
                    label: "Date",
                    render: (row) => formatDateTime(row.createdAt),
                  },
                ]}
                data={selectedTask.statusHistory || []}
                emptyMessage="No status history available."
                getRowKey={(row) => row.taskStatusHistoryId}
              />
            </div>

            {/* Assignment History */}
            <div>
              <h3 className="font-semibold text-slate-800 mb-3">
                Assignment History
              </h3>

              <DataTable
                columns={[
                  {
                    key: "oldAssignedStaffName",
                    label: "Previous Staff",
                    render: (row) => row.oldAssignedStaffName || "Initial",
                  },
                  {
                    key: "newAssignedStaffName",
                    label: "New Staff",
                  },
                  {
                    key: "remarks",
                    label: "Remarks",
                    render: (row) => row.remarks || "-",
                  },
                  {
                    key: "createdAt",
                    label: "Date",
                    render: (row) => formatDateTime(row.createdAt),
                  },
                ]}
                data={selectedTask.assignmentHistory || []}
                emptyMessage="No assignment history available."
                getRowKey={(row) => row.taskAssignmentHistoryId}
              />
            </div>

            {/* Actions */}
            <div className="flex justify-end border-t border-slate-200 pt-4">
              <button
                type="button"
                onClick={handleOpenUpdateModal}
                className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg text-sm font-medium"
              >
                Update Status
              </button>
            </div>

          </div>
        ) : null}
      </Modal>

      {/* Update Status Modal */}
      <Modal
        isOpen={showUpdateModal}
        onClose={() => {
          if (!saving) setShowUpdateModal(false);
        }}
        title="Update Task Status"
        size="lg"
      >
        <form onSubmit={handleUpdateStatus} className="space-y-4">

          {updateError && (
            <div className="bg-red-50 text-red-700 border border-red-200 rounded-lg p-3 text-sm">
              {updateError}
            </div>
          )}

          {statusError && (
            <div className="bg-red-50 text-red-700 border border-red-200 rounded-lg p-3 text-sm">
              {statusError}
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-2">
              New Status
            </label>

            <select
              value={newStatusId}
              onChange={(e) => setNewStatusId(e.target.value)}
              required
              className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
            >
              <option value="">Select Status</option>

              {statuses.map((status) => (
                <option
                  key={status.taskStatusId}
                  value={status.taskStatusId}
                >
                  {status.taskStatusName}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-2">
              Remarks
            </label>

            <textarea
              value={remarks}
              onChange={(e) => setRemarks(e.target.value)}
              rows={3}
              placeholder="Add remarks about this status update..."
              className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-2">
              Completion Note
            </label>

            <textarea
              value={completionNote}
              onChange={(e) => setCompletionNote(e.target.value)}
              rows={3}
              placeholder="Add or update the completion note..."
              className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
            />
          </div>

          <div className="flex justify-end gap-3 pt-3">
            <button
              type="button"
              onClick={() => setShowUpdateModal(false)}
              disabled={saving}
              className="px-4 py-2 border border-slate-300 rounded-lg text-sm hover:bg-slate-50 disabled:opacity-50"
            >
              Cancel
            </button>

            <button
              type="submit"
              disabled={saving || statuses.length === 0}
              className="px-5 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-medium disabled:opacity-50"
            >
              {saving ? "Updating..." : "Save Changes"}
            </button>
          </div>

        </form>
      </Modal>

    </div>
  );
}

export default MyTasks;
