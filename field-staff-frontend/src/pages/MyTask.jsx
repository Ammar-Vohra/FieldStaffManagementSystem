
import { useEffect, useState } from "react";

import serviceTaskService from "../services/serviceTaskService";
import taskStatusService from "../services/taskStatusService";
import taskPriorityService from "../services/taskPriorityService";

import DataTable from "../components/common/DataTable";
import Modal from "../components/common/Modal";
import StatusBadge from "../components/common/StatusBadge";
import Pagination from "../components/data-display/Pagination";
import SearchInput from "../components/filters/SearchInput";
import SelectFilter from "../components/filters/SelectFilter";
import SortSelect from "../components/filters/SortSelect";
import FormInput from "../components/common/FormInput";

function MyTasks() {
  const [tasks, setTasks] = useState([]);
  const [statuses, setStatuses] = useState([]);
  const [priorities, setPriorities] = useState([]);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [statusError, setStatusError] = useState("");

  // Pagination and search
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [priorityFilter, setPriorityFilter] = useState("");
  const [scheduledDateFrom, setScheduledDateFrom] = useState("");
  const [scheduledDateTo, setScheduledDateTo] = useState("");
  const [sortBy, setSortBy] = useState("scheduledDate");
  const [sortDirection, setSortDirection] = useState("desc");

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

  // Normalize status code/name
  const normalizeStatus = (value) => {
    return (value || "")
      .trim()
      .toUpperCase()
      .replace(/\s+/g, "_");
  };

  // Get next allowed status
  const getNextStatus = (task) => {
    if (!task) return null;

    const currentStatus = normalizeStatus(
      task.taskStatusCode || task.taskStatusName
    );

    let nextStatusCode = null;

    // Pending and Overdue tasks can both be started.
    if (
      currentStatus === "PENDING" ||
      currentStatus === "OVERDUE"
    ) {
      nextStatusCode = "IN_PROGRESS";
    } else if (currentStatus === "IN_PROGRESS") {
      nextStatusCode = "COMPLETED";
    }

    if (!nextStatusCode) return null;

    return (
      statuses.find((status) => {
        const statusCode = normalizeStatus(
          status.taskStatusCode || status.taskStatusName
        );

        return statusCode === nextStatusCode;
      }) || null
    );
  };

  // Get next action button label
  const getNextActionLabel = (task) => {
    const currentStatus = normalizeStatus(
      task?.taskStatusCode || task?.taskStatusName
    );

    if (
      currentStatus === "PENDING" ||
      currentStatus === "OVERDUE"
    ) {
      return "Start Task";
    }

    if (currentStatus === "IN_PROGRESS") {
      return "Complete Task";
    }

    return "Update Status";
  };

  // Load assigned tasks
  const fetchMyTasks = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await serviceTaskService.getMyTasks({
        pageNumber,
        pageSize,
        search,
        taskStatusId: statusFilter
          ? Number(statusFilter)
          : undefined,
        taskPriorityId: priorityFilter
          ? Number(priorityFilter)
          : undefined,
        scheduledDateFrom: scheduledDateFrom || undefined,
        scheduledDateTo: scheduledDateTo || undefined,
        sortBy,
        sortDirection,
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
      setStatusError("");

      const response = await taskStatusService.getAllTaskStatuses();
      setStatuses(response || []);
    } catch (err) {
      setStatusError(
        err.response?.data?.message ||
          "Unable to load task statuses."
      );
    }
  };

  // Load available task priorities
  const fetchPriorities = async () => {
    try {
      const response = await taskPriorityService.getAllTaskPriorities();
      setPriorities(response || []);
    } catch (err) {
      setStatusError(
        err.response?.data?.message ||
          "Unable to load task priorities."
      );
    }
  };

  useEffect(() => {
    fetchMyTasks();
  }, [
    pageNumber,
    pageSize,
    search,
    statusFilter,
    priorityFilter,
    scheduledDateFrom,
    scheduledDateTo,
    sortBy,
    sortDirection,
  ]);

  useEffect(() => {
    fetchStatuses();
    fetchPriorities();
  }, []);

  // Search
  const handleSearch = (e) => {
    setSearch(e.target.value);
    setPageNumber(1);
  };

  // Reset filters
  const handleResetFilters = () => {
    setSearch("");
    setStatusFilter("");
    setPriorityFilter("");
    setScheduledDateFrom("");
    setScheduledDateTo("");
    setSortBy("scheduledDate");
    setSortDirection("desc");
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

    const nextStatus = getNextStatus(selectedTask);

    if (!nextStatus) {
      setUpdateError("No further status update is available.");
      return;
    }

    setNewStatusId(String(nextStatus.taskStatusId));
    setRemarks("");
    setCompletionNote("");
    setUpdateError("");
    setShowUpdateModal(true);
  };

  // Update task status
  const handleUpdateStatus = async (e) => {
    e.preventDefault();

    if (!newStatusId) {
      setUpdateError("Unable to determine the next task status.");
      return;
    }

    const targetStatus = statuses.find(
      (status) => status.taskStatusId === Number(newStatusId)
    );

    if (!targetStatus) {
      setUpdateError("Selected task status is invalid.");
      return;
    }

    const targetStatusCode = normalizeStatus(
      targetStatus.taskStatusCode || targetStatus.taskStatusName
    );

    // Validate completion note
    if (
      targetStatusCode === "COMPLETED" &&
      !completionNote.trim()
    ) {
      setUpdateError(
        "Completion note is required when completing a task."
      );
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
          completionNote:
            targetStatusCode === "COMPLETED"
              ? completionNote.trim()
              : null,
        }
      );

      setSelectedTask(response);
      setShowUpdateModal(false);

      // Refresh task list
      await fetchMyTasks();

      // Refresh task details and status history
      const updatedDetails =
        await serviceTaskService.getMyTaskDetails(
          selectedTask.serviceTaskId
        );

      setSelectedTask(updatedDetails);
    } catch (err) {
      setUpdateError(
        err.response?.data?.message ||
          "Unable to update task status."
      );
    } finally {
      setSaving(false);
    }
  };

  // Format date
  const formatDate = (date) => {
    if (!date) return "-";

    return new Date(date).toLocaleDateString("en-PK", {
      day: "2-digit",
      month: "short",
      year: "numeric",
    });
  };

  // Format date and time
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
      render: (row) => (
        <StatusBadge status={row.taskStatusName} />
      ),
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

      {/* Search, filters and sorting */}
      <div className="bg-white border border-slate-200 rounded-xl p-4 md:p-5 space-y-4">
        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
          <SearchInput
            label="Search Tasks"
            value={search}
            onChange={handleSearch}
            placeholder="Search title, customer, or description..."
          />

          <SelectFilter
            label="Status"
            value={statusFilter}
            onChange={(event) => {
              setStatusFilter(event.target.value);
              setPageNumber(1);
            }}
            placeholder="All statuses"
            options={statuses
              .filter((status) => status.isActive !== false)
              .map((status) => ({
                value: String(status.taskStatusId),
                label: status.taskStatusName,
              }))}
          />

          <SelectFilter
            label="Priority"
            value={priorityFilter}
            onChange={(event) => {
              setPriorityFilter(event.target.value);
              setPageNumber(1);
            }}
            placeholder="All priorities"
            options={priorities
              .filter((priority) => priority.isActive !== false)
              .map((priority) => ({
                value: String(priority.taskPriorityId),
                label: priority.taskPriorityName,
              }))}
          />

          <SortSelect
            label="Sort By"
            value={`${sortBy}|${sortDirection}`}
            onChange={(event) => {
              const [field, direction] = event.target.value.split("|");
              setSortBy(field);
              setSortDirection(direction);
              setPageNumber(1);
            }}
            options={[
              { value: "scheduledDate|asc", label: "Schedule: soonest first" },
              { value: "scheduledDate|desc", label: "Schedule: latest first" },
              { value: "priority|asc", label: "Priority: A to Z" },
              { value: "priority|desc", label: "Priority: Z to A" },
              { value: "title|asc", label: "Task title: A to Z" },
              { value: "title|desc", label: "Task title: Z to A" },
              { value: "createdAt|desc", label: "Recently added" },
              { value: "createdAt|asc", label: "Oldest added" },
            ]}
          />

          <FormInput
            label="Scheduled From"
            name="scheduledDateFrom"
            type="date"
            value={scheduledDateFrom}
            onChange={(event) => {
              setScheduledDateFrom(event.target.value);
              setPageNumber(1);
            }}
          />

          <FormInput
            label="Scheduled To"
            name="scheduledDateTo"
            type="date"
            value={scheduledDateTo}
            onChange={(event) => {
              setScheduledDateTo(event.target.value);
              setPageNumber(1);
            }}
          />
        </div>

        <div className="flex justify-end border-t border-slate-100 pt-3">
          <button
            type="button"
            onClick={handleResetFilters}
            className="px-4 py-2 text-sm font-medium text-slate-600 border border-slate-300 rounded-lg hover:bg-slate-50 transition"
          >
            Reset Filters
          </button>
        </div>
      </div>

      {/* Error */}
      {error && (
        <div className="bg-red-50 text-red-700 border border-red-200 rounded-lg p-3 text-sm">
          {error}
        </div>
      )}

      {/* Status loading error */}
      {statusError && (
        <div className="bg-red-50 text-red-700 border border-red-200 rounded-lg p-3 text-sm">
          {statusError}
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
        onClose={() => {
          if (!showUpdateModal && !saving) {
            setShowDetailsModal(false);
          }
        }}
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
                <StatusBadge status={selectedTask.taskStatusName} />
              </div>

              <div>
                <p className="text-xs text-slate-500">Record State</p>
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
                    render: (row) => (
                      <StatusBadge status={row.newStatusName} />
                    ),
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
              {getNextStatus(selectedTask) ? (
                <button
                  type="button"
                  onClick={handleOpenUpdateModal}
                  disabled={statuses.length === 0 || saving}
                  className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg text-sm font-medium disabled:opacity-50"
                >
                  {getNextActionLabel(selectedTask)}
                </button>
              ) : (
                <span className="text-sm text-slate-500">
                  No further status updates available.
                </span>
              )}
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

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-2">
              Current Status
            </label>

            <div className="w-full border border-slate-200 bg-slate-50 rounded-lg px-3 py-2 text-sm text-slate-700">
              <StatusBadge status={selectedTask?.taskStatusName} />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-2">
              New Status
            </label>

            <div className="w-full border border-slate-200 bg-slate-50 rounded-lg px-3 py-2 text-sm text-slate-700">
              <StatusBadge
                status={
                  statuses.find(
                    (status) =>
                      status.taskStatusId === Number(newStatusId)
                  )?.taskStatusName || "Loading status..."
                }
              />
            </div>
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

          {normalizeStatus(
            statuses.find(
              (status) =>
                status.taskStatusId === Number(newStatusId)
            )?.taskStatusCode ||
              statuses.find(
                (status) =>
                  status.taskStatusId === Number(newStatusId)
              )?.taskStatusName
          ) === "COMPLETED" && (
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-2">
                Completion Note <span className="text-red-500">*</span>
              </label>

              <textarea
                value={completionNote}
                onChange={(e) => setCompletionNote(e.target.value)}
                rows={3}
                required
                placeholder="Describe the work completed..."
                className="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
              />
            </div>
          )}

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
              disabled={saving || !newStatusId || statuses.length === 0}
              className="px-5 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-medium disabled:opacity-50"
            >
              {saving ? "Updating..." : "Confirm Status Update"}
            </button>
          </div>

        </form>
      </Modal>

    </div>
  );
}

export default MyTasks;
