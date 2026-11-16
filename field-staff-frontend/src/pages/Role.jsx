
import { useEffect, useState } from "react";

import roleService from "../services/roleService";

import Modal from "../components/common/Modal";
import ConfirmDialog from "../components/common/ConfirmDialogue";
import FormInput from "../components/common/FormInput";
import StatusBadge from "../components/common/StatusBadge";
import DataTable from "../components/common/DataTable";

function Roles() {
  // Roles data
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(true);

  // Modal states
  const [showModal, setShowModal] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);

  // Selected role
  const [selectedRole, setSelectedRole] = useState(null);

  // Form states
  const [roleName, setRoleName] = useState("");
  const [isActive, setIsActive] = useState(true);
  const [formError, setFormError] = useState("");

  // Operation states
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);

  // General messages
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  // Fetch all roles
  const fetchRoles = async () => {
    try {
      setLoading(true);
      setError("");

      const data = await roleService.getAllRoles();

      setRoles(data);
    } catch (err) {
      console.error("Failed to fetch roles:", err);

      setError(
        err.response?.data?.message ||
        err.response?.data?.title ||
        "Unable to load roles."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoles();
  }, []);

  // Reset form
  const resetForm = () => {
    setRoleName("");
    setIsActive(true);
    setSelectedRole(null);
    setFormError("");
  };

  // Open Add Role Modal
  const handleAdd = () => {
    resetForm();
    setShowModal(true);
  };

  // Open Edit Role Modal
  const handleEdit = (role) => {
    setSelectedRole(role);
    setRoleName(role.roleName);
    setIsActive(role.isActive);
    setFormError("");
    setShowModal(true);
  };

  // Close Modal
  const handleCloseModal = () => {
    if (saving) return;

    setShowModal(false);
    resetForm();
  };

  // Save Role (Create / Update)
  const handleSubmit = async (e) => {
    e.preventDefault();

    setFormError("");
    setError("");
    setSuccess("");

    const trimmedName = roleName.trim();

    if (trimmedName.length < 2 || trimmedName.length > 100) {
      setFormError(
        "Role name must contain between 2 and 100 characters."
      );
      return;
    }

    try {
      setSaving(true);

      if (selectedRole) {
        await roleService.updateRole(selectedRole.id, {
          roleName: trimmedName,
          isActive: isActive,
        });

        setSuccess("Role updated successfully.");
      } else {
        await roleService.createRole({
          roleName: trimmedName,
        });

        setSuccess("Role created successfully.");
      }

      setShowModal(false);
      resetForm();

      await fetchRoles();
    } catch (err) {
      console.error("Failed to save role:", err);

      setFormError(
        err.response?.data?.message ||
        err.response?.data?.title ||
        "Unable to save role."
      );
    } finally {
      setSaving(false);
    }
  };

  // Open Delete Confirmation
  const handleDeleteClick = (role) => {
    setSelectedRole(role);
    setShowDeleteDialog(true);
  };

  // Delete Role
  const handleDelete = async () => {
    if (!selectedRole) return;

    try {
      setDeleting(true);
      setError("");
      setSuccess("");

      await roleService.deleteRole(selectedRole.id);

      setSuccess("Role deleted successfully.");

      setShowDeleteDialog(false);
      setSelectedRole(null);

      await fetchRoles();
    } catch (err) {
      console.error("Failed to delete role:", err);

      setError(
        err.response?.data?.message ||
        err.response?.data?.title ||
        "Unable to delete role."
      );
    } finally {
      setDeleting(false);
    }
  };

  // Table Columns
  const columns = [
    {
      key: "id",
      label: "Role ID",
    },
    {
      key: "roleName",
      label: "Role Name",
    },
    {
      key: "isActive",
      label: "Status",
      render: (role) => (
        <StatusBadge isActive={role.isActive} />
      ),
    },
    {
      key: "actions",
      label: "Actions",
      render: (role) => (
        <div className="flex items-center gap-3">

            <button
                onClick={() => handleEdit(role)}
                className="px-3 py-1.5 text-sm font-medium text-blue-700 bg-blue-100 hover:bg-blue-200 rounded-lg transition"
            >
                Edit
            </button>

            <button
                onClick={() => handleDeleteClick(role)}
                className="px-3 py-1.5 text-sm font-medium text-red-700 bg-red-100 hover:bg-red-200 rounded-lg transition"
            >
                Delete
            </button>

        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">

      {/* Page Header */}
      <div className="flex items-center justify-between">

        <div>
          <h1 className="text-2xl font-bold text-slate-800">
            Role Management
          </h1>

          <p className="text-slate-500 mt-1">
            Manage application roles and their status.
          </p>
        </div>

        <button
          onClick={handleAdd}
          className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition"
        >
          + Add Role
        </button>

      </div>

      {/* Success Message */}
      {success && (
        <div className="bg-green-100 text-green-700 px-4 py-3 rounded-lg">
          {success}
        </div>
      )}

      {/* Error Message */}
      {error && (
        <div className="bg-red-100 text-red-700 px-4 py-3 rounded-lg">
          {error}
        </div>
      )}

      {/* Roles Table */}
      <DataTable
        columns={columns}
        data={roles}
        loading={loading}
        emptyMessage="No roles found."
        getRowKey={(role) => role.id}
      />

      {/* Add / Edit Modal */}
      <Modal
        isOpen={showModal}
        onClose={handleCloseModal}
        title={selectedRole ? "Edit Role" : "Add New Role"}
        size="md"
      >

        <form onSubmit={handleSubmit} className="space-y-5">

          <FormInput
            label="Role Name"
            name="roleName"
            value={roleName}
            onChange={(e) => setRoleName(e.target.value)}
            placeholder="Enter role name"
            required
            error={formError}
            disabled={saving}
          />

          {/* Active Status (Edit only) */}
          {selectedRole && (
            <label className="flex items-center gap-2 cursor-pointer">

              <input
                type="checkbox"
                checked={isActive}
                onChange={(e) => setIsActive(e.target.checked)}
                disabled={saving}
                className="w-4 h-4 accent-blue-600"
              />

              <span className="text-sm text-slate-700">
                Active
              </span>

            </label>
          )}

          {/* Form Actions */}
          <div className="flex justify-end gap-3 pt-3 border-t">

            <button
              type="button"
              onClick={handleCloseModal}
              disabled={saving}
              className="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-100"
            >
              Cancel
            </button>

            <button
              type="submit"
              disabled={saving}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
            >
              {saving
                ? "Saving..."
                : selectedRole
                  ? "Update Role"
                  : "Create Role"}
            </button>

          </div>

        </form>

      </Modal>

      {/* Delete Confirmation */}
      <ConfirmDialog
        isOpen={showDeleteDialog}
        onClose={() => {
          if (!deleting) {
            setShowDeleteDialog(false);
            setSelectedRole(null);
          }
        }}
        onConfirm={handleDelete}
        title="Delete Role"
        message={`Are you sure you want to delete "${selectedRole?.roleName}"?`}
        confirmText="Delete"
        loading={deleting}
      />

    </div>
  );
}

export default Roles;
