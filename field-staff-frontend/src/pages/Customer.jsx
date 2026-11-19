import { useEffect, useState, useCallback } from "react";

import customerService from "../services/customerService";
import Modal from "../components/common/Modal";
import ConfirmDialog from "../components/common/ConfirmDialogue";
import StatusBadge from "../components/common/StatusBadge";
import DataTable from "../components/common/DataTable";
import GenericForm from "../components/common/GenericForm";
import SearchInput from "../components/filters/SearchInput";
import SelectFilter from "../components/filters/SelectFilter";
import SortSelect from "../components/filters/SortSelect";
import Pagination from "../components/data-display/Pagination";


function Customers () {

    const [customers, setCustomers] = useState([]);

    // Loading
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);

    // Messages
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    // Modal
    const [showModal, setShowModal] = useState(false);
    const [showDeleteDialog, setShowDeleteDialog] = useState(false);

    // Selected Customer
    const [selectedCustomer, setSelectedCustomer] = useState(null);

    // Paginaton
    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalRecords, setTotalRecords] = useState(0);
    const [hasPreviousPage, setHasPreviousPage] = useState(false);
    const [hasNextPage, setHasNextPage] = useState(false);

    // Search, filter & Sort
    const [search, setSearch] = useState("");
    const [statusFilter, setStatusFilter] = useState("");
    const [sortBy, setSortBy] = useState("customerName");
    const [sortDirection, setSortDirection] = useState("asc");


    const fetchCustomers = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const result = await customerService.getPagedCustomers({
                pageNumber,
                pageSize,
                search,
                sortBy,
                sortDirection,
                isActive : statusFilter == ""
                    ? undefined
                    : statusFilter == "true"

            });

            const customerData = result.data || [];

            if (
                customerData.length == 0 &&
                pageNumber > 1 &&
                pageNumber > result.totalPages
            ) {
                setPageNumber(result.totalPages || 1);
                return; 
            }

            setCustomers(customerData);
            setTotalPages(result.totalPages);
            setTotalRecords(result.totalRecords);
            setHasPreviousPage(result.hasPreviousPage);
            setHasNextPage(result.hasNextPage);

        } catch (err) {
            console.error("Failed to fetch customers: ", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Unable to load Customers."
            );
        } finally {
            setLoading(false);
        }

    }, [
        pageNumber,
        pageSize,
        search,
        statusFilter,
        sortBy,
        sortDirection
    ]);


    useEffect(() => {
        fetchCustomers();
    }, [fetchCustomers]);

    const resetForm = () => {
        setSelectedCustomer(null);
    };

    const handleAdd = () => {
        resetForm();
        setError("");
        setSuccess("");
        setShowModal(true);
    };

    const handleEdit = (customer) => {
        setSelectedCustomer(customer);
        setError("");
        setSuccess("");
        setShowModal(true);
    };

    const handleClose = () => {
        if (saving) return;

        setShowModal(false);
        resetForm();
    };

    const customerfields = [
        {
            name: "customerName",
            label: "CustomerName",
            type: "text",
            placeholder: "Enter customer name",
            required: true,
            minLength: 3,
            maxLength: 100
        },
        {
            name: "email",
            label: "Email",
            type: "email",
            placeholder: "Enter email address",
            required: true,
            maxLength: 30
        },
        {
            name: "serviceAddress",
            label: "ServiceAddress",
            type: "text",
            placeholder: "Enter address",
            required: true,
            minLength: 3,
            maxLength: 100
        },
        {
            name: "isActive",
            label: "Active",
            type: "checkbox"
        }
    ];


    const InitialFormData = selectedCustomer
        ? {
            customerName: selectedCustomer.customerName,
            email: selectedCustomer.email,
            serviceAddress: selectedCustomer.serviceAddress,
            isActive: selectedCustomer.isActive
        }
        : {
            customerName: "",
            email: "",
            serviceAddress: "",
            isActive: true

        };

            const handleSubmit = async (formData, setFormErrors) => {
                setError("");
                setSuccess("");
                setFormErrors({});

                const customerData = {
                    customerName: (formData.customerName ?? "").trim(),
                    email: (formData.email ?? "").trim(),
                    serviceAddress: (formData.serviceAddress ?? "").trim(),
                    isActive: formData.isActive ?? true,

                };

                try {
                    setSaving(true);

                    if (selectedCustomer) {
                        await customerService.updateCustomer(selectedCustomer.customerId, customerData);
                    
                    setSuccess("Customer updated successfully.");
                    
                    } else {
                        await customerService.createCustomer({
                            ...customerData
                        });

                        setSuccess("Customer created successfully.");
                    }

                    setShowModal(false);
                    resetForm();

                    await fetchCustomers();
                
                } catch (err) {
                    console.error("Failed to save customer: ", err)

                    setFormErrors({
                        general: 
                            err.response?.data?.message ||
                            err.response?.data?.title ||
                            "Unable to save customer."
                    });

                } finally {
                    setSaving(false);
                }
            };


            const handleDelete = async () => {
                if (!selectedCustomer) return;

                try {
                    setDeleting(true);
                    setError("");
                    setSuccess("");

                    await customerService.deleteCustomer(selectedCustomer.customerId);

                    setSuccess("Customer deleted successfully.");
                    setShowDeleteDialog(false);

                    if (customers.length === 1 && pageNumber > 1) {
                        setPageNumber(prev => prev - 1);
                    } else {
                        await fetchCustomers();
                    }

                    setSelectedCustomer(null);
                
                } catch (err) {
                    console.error("Failed to delete customer: ", err)

                    setError(
                        err.response?.data?.message ||
                        err.response?.data?.title ||
                        "Unable to delete customer."
                    );

                } finally {
                    setDeleting(false);
                }
            };

            const columns = [
                {
                    key: "customerId",
                    label: "Customer ID"
                },
                {
                    key: "customerName",
                    label: "Customer Name"
                },
                {
                    key: "email",
                    label: "Email"
                },
                {
                    key: "serviceAddress",
                    label: "Service Address"
                },
                {
                    key: "isActive",
                    label: "Status",
                    render: (customer) => (
                        <StatusBadge isActive={customer.isActive} />
                    )
                },
                {
                    key: "actions",
                    label: "Actions",
                    render: (customer) => (
                        <div className="flex items-center gap-3">
                            <button
                                onClick={() => handleEdit(customer)}
                                className="px-3 py-1.5 text-sm font-medium text-blue-700 bg-blue-100 hover:bg-blue-200 rounded-lg transition"
                            >
                                Edit
                            </button>

                            <button
                                onClick={() => {
                                    setSelectedCustomer(customer);
                                    setShowDeleteDialog(true);
                                }}
                                className="px-3 py-1.5 text-sm font-medium text-red-700 bg-red-100 hover:bg-red-200 rounded-lg transition"

                            >
                                Delete
                            </button>

                        </div>
                    )
                },

            ];
    
return (
    <div className="space-y-6">
        <div className="flex items-center justify-between">

            <div>
                <h1 className="text-2xl font-bold text-slate-800">
                    Customer Management
                </h1>

                <p className="text-slate-500 mt-1">
                    Manage application Customers .
                </p>
            </div>

            <button
                onClick={handleAdd}
                className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition"
            >
                + Add Customer
            </button>

        </div>


        {success && (
            <div className="bg-green-100 text-green-700 px-4 py-3 rounded-lg">
                    {success}
            </div>
        )}

         
        {error && (
            <div className="bg-red-100 text-red-700 px-4 py-3 rounded-lg">
                    {error}
            </div>
        )}

        <div className="bg-white p-5 rounded-xl border border-slate-200">
            
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">

                <SearchInput
                    label="Search Customers"
                    value={search}
                    onChange={(e) => {
                        setSearch(e.target.value);
                        setPageNumber(1);
                    }} 
                    placeholder="Search customer name or email..."
                />

                <SelectFilter
                    label="Filter by Status"
                    value={statusFilter}
                    onChange={(e) => {
                        setStatusFilter(e.target.value);
                        setPageNumber(1);
                    }}
                    placeholder="All Statuses"
                    options={[
                        {value: "true", label: "Active"},
                        {value: "false", label: "Inactive"}
                    ]}
                />

                <SortSelect 
                    label="Sort By"
                    value={`${sortBy} | ${sortDirection}`}
                    onChange={(e) => {
                        const [column, direction] = e.target.value.split("|");
                        setSortBy(column);
                        setSortDirection(direction);
                        setPageNumber(1);
                    }}

                    options={[
                        { value: "customerName|asc", label: "CustomerName (A-Z)" },
                        { value: "customerName|desc", label: "CustomerName (Z-A)" },
                        { value: "email|asc", label: "Email (A-Z)" },
                        { value: "email|desc", label: "Email (Z-A)" },
                        { value: "createdAt|desc", label: "Newest First" },
                        { value: "createdAt|asc", label: "Oldest First" },
                        { value: "isActive|asc", label: "Status (Ascending)" },
                        { value: "isActive|desc", label: "Status (Descending)" }
                    ]}
                />

            </div>

        </div>

        
        <DataTable
            columns={columns}
            data={customers}
            loading={loading}
            emptyMessage="No Customers found."
            getRowKey={(customer) => customer.customerId}
        />

        <Pagination
            pageNumber={pageNumber}
            pageSize={pageSize}
            totalPages={totalPages}
            totalRecords={totalRecords}
            hasPreviousPage={hasPreviousPage}
            hasNextPage={hasNextPage}
            onPageChange={setPageNumber}
            onPageSizeChange={(size) => {
                setPageSize(size);
                setPageNumber(1);
            }}
            
            loading={loading}
        />

        <Modal
            isOpen={showModal}
            onClose={handleClose}
            title={selectedCustomer ? "Edit Customer" : "Add New Customer"}
        >
            <GenericForm
                key={selectedCustomer?.customerId ?? "new-customer"}
                fields={customerfields}
                initialData={InitialFormData}
                saving={saving}
                submitText={selectedCustomer ? "Update Customer" : "Create Customer"}
                onSubmit={handleSubmit}
                onCancel={handleClose}
            />
        </Modal>

        <ConfirmDialog
            isOpen={showDeleteDialog}
            onClose={() => {
                if (!deleting) {
                    setShowDeleteDialog(false);
                    setSelectedCustomer(null);
                }
            }}
            onConfirm={handleDelete}
            title="Delete Customer"
            message={`Are you sure you want to delete "${selectedCustomer?.customerName}"?`}
            confirmText="Delete"
            loading = {deleting}
        />

    </div>
);
    
}

export default Customers;