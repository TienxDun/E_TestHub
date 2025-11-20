// User Management JavaScript
$(document).ready(function () {
    // Edit user button click
    $(document).on('click', '.btn-edit', function () {
        const userId = $(this).data('user-id');
        if (userId) {
            window.location.href = `/Admin/EditUser/${userId}`;
        }
    });

    // Delete user button click
    $(document).on('click', '.btn-delete', function () {
        const userId = $(this).data('user-id');
        const row = $(this).closest('tr');
        const userName = row.find('.user-name').text();

        if (confirm(`Bạn có chắc chắn muốn xóa người dùng "${userName}"?\n\nHành động này sẽ vô hiệu hóa tài khoản người dùng.`)) {
            deleteUser(userId, row);
        }
    });

    // Delete user function
    function deleteUser(userId, row) {
        $.ajax({
            url: '/Admin/DeleteUser/' + userId,
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            beforeSend: function() {
                // Show loading state
                row.css('opacity', '0.5');
            },
            success: function (response) {
                if (response.success) {
                    // Show success message
                    showNotification('success', response.message);
                    
                    // Remove row with animation
                    row.fadeOut(400, function() {
                        $(this).remove();
                        updateTableInfo();
                    });
                } else {
                    // Show error message
                    showNotification('error', response.message);
                    row.css('opacity', '1');
                }
            },
            error: function () {
                showNotification('error', 'Đã xảy ra lỗi khi xóa người dùng. Vui lòng thử lại.');
                row.css('opacity', '1');
            }
        });
    }

    // View user details
    $(document).on('click', '.btn-view', function () {
        const userId = $(this).data('user-id');
        const row = $(this).closest('tr');
        
        // Get user data from row
        const userData = {
            id: userId,
            fullName: row.find('.user-name').text(),
            userId: row.find('.user-id').text(),
            email: row.find('td:eq(2)').text(),
            role: row.find('.role-badge').text().trim(),
            status: row.find('.status-badge').text().trim(),
            createdDate: row.find('td:eq(5)').text(),
            lastLogin: row.find('td:eq(6)').text()
        };

        // Populate modal
        $('#detailId').text(userData.id);
        $('#detailFullName').text(userData.fullName);
        $('#detailEmail').text(userData.email);
        $('#detailRole').html(`<span class="role-badge">${userData.role}</span>`);
        $('#detailStatus').html(`<span class="status-badge">${userData.status}</span>`);
        $('#detailCreatedDate').text(userData.createdDate);
        $('#detailLastLogin').text(userData.lastLogin);
        
        // Additional info based on role
        let additionalInfo = '';
        if (userData.role.includes('Sinh Viên')) {
            additionalInfo = `<p><strong>Mã sinh viên:</strong> ${userData.userId}</p>`;
        } else if (userData.role.includes('Giáo Viên')) {
            additionalInfo = `<p><strong>Mã giảng viên:</strong> ${userData.userId}</p>`;
        } else if (userData.role.includes('Quản Trị')) {
            additionalInfo = `<p><strong>Mã nhân viên:</strong> ${userData.userId}</p>`;
        }
        $('#additionalInfo').html(additionalInfo);

        // Store user ID for edit button
        $('#editUserBtn').data('user-id', userId);

        // Show modal
        $('#userDetailsModal').modal('show');
    });

    // Edit from modal
    $('#editUserBtn').on('click', function() {
        const userId = $(this).data('user-id');
        window.location.href = `/Admin/EditUser/${userId}`;
    });

    // Activate/Deactivate user
    $(document).on('click', '.btn-activate, .btn-deactivate', function () {
        const userId = $(this).data('user-id');
        const row = $(this).closest('tr');
        const isActivate = $(this).hasClass('btn-activate');
        const action = isActivate ? 'kích hoạt' : 'vô hiệu hóa';
        const userName = row.find('.user-name').text();

        if (confirm(`Bạn có chắc chắn muốn ${action} tài khoản "${userName}"?`)) {
            toggleUserStatus(userId, isActivate, row);
        }
    });

    // Toggle user status function
    function toggleUserStatus(userId, activate, row) {
        // Note: Backend API needs this endpoint
        $.ajax({
            url: `/Admin/ToggleUserStatus/${userId}`,
            type: 'POST',
            data: { isActive: activate },
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            beforeSend: function() {
                row.css('opacity', '0.5');
            },
            success: function (response) {
                if (response.success) {
                    // Update status badge
                    const statusBadge = row.find('.status-badge');
                    if (activate) {
                        statusBadge.removeClass('status-inactive').addClass('status-active').text('Hoạt Động');
                    } else {
                        statusBadge.removeClass('status-active').addClass('status-inactive').text('Không Hoạt Động');
                    }

                    // Update action button
                    const actionBtn = row.find('.btn-activate, .btn-deactivate');
                    if (activate) {
                        actionBtn.removeClass('btn-activate').addClass('btn-deactivate')
                            .attr('title', 'Vô hiệu hóa')
                            .html('<i class="fas fa-pause"></i>');
                    } else {
                        actionBtn.removeClass('btn-deactivate').addClass('btn-activate')
                            .attr('title', 'Kích hoạt')
                            .html('<i class="fas fa-play"></i>');
                    }

                    showNotification('success', response.message);
                } else {
                    showNotification('error', response.message);
                }
                row.css('opacity', '1');
            },
            error: function () {
                showNotification('error', 'Đã xảy ra lỗi. Vui lòng thử lại.');
                row.css('opacity', '1');
            }
        });
    }

    // Select all checkboxes
    $('#selectAll').on('change', function () {
        $('.user-checkbox').prop('checked', $(this).prop('checked'));
        updateBulkActionButtons();
    });

    // Individual checkbox change
    $(document).on('change', '.user-checkbox', function () {
        updateBulkActionButtons();
        
        // Update select all checkbox
        const totalCheckboxes = $('.user-checkbox').length;
        const checkedCheckboxes = $('.user-checkbox:checked').length;
        $('#selectAll').prop('checked', totalCheckboxes === checkedCheckboxes);
    });

    // Update bulk action buttons state
    function updateBulkActionButtons() {
        const checkedCount = $('.user-checkbox:checked').length;
        $('#bulkActivate, #bulkDeactivate, #bulkDelete').prop('disabled', checkedCount === 0);
    }

    // Bulk delete
    $('#bulkDelete').on('click', function () {
        const selectedIds = $('.user-checkbox:checked').map(function () {
            return $(this).val();
        }).get();

        if (selectedIds.length === 0) return;

        if (confirm(`Bạn có chắc chắn muốn xóa ${selectedIds.length} người dùng đã chọn?`)) {
            bulkDeleteUsers(selectedIds);
        }
    });

    // Bulk delete function
    function bulkDeleteUsers(userIds) {
        let successCount = 0;
        let errorCount = 0;

        userIds.forEach((userId, index) => {
            $.ajax({
                url: `/Admin/DeleteUser/${userId}`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.success) {
                        successCount++;
                        $(`tr[data-user-id="${userId}"]`).fadeOut(400, function() {
                            $(this).remove();
                        });
                    } else {
                        errorCount++;
                    }

                    // Show result after all requests complete
                    if (index === userIds.length - 1) {
                        if (successCount > 0) {
                            showNotification('success', `Đã xóa thành công ${successCount} người dùng.`);
                        }
                        if (errorCount > 0) {
                            showNotification('error', `Không thể xóa ${errorCount} người dùng.`);
                        }
                        $('#selectAll').prop('checked', false);
                        updateBulkActionButtons();
                        updateTableInfo();
                    }
                },
                error: function () {
                    errorCount++;
                }
            });
        });
    }

    // Search functionality
    let searchTimeout;
    $('#searchInput').on('keyup', function () {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(function () {
            filterTable();
        }, 300);
    });

    $('#clearSearch').on('click', function () {
        $('#searchInput').val('');
        filterTable();
    });

    // Apply filters
    $('#applyFilters').on('click', function () {
        filterTable();
    });

    // Reset filters
    $('#resetFilters').on('click', function () {
        $('#searchInput').val('');
        $('#roleFilter').val('');
        $('#statusFilter').val('');
        filterTable();
    });

    // Filter table function
    function filterTable() {
        const searchTerm = $('#searchInput').val().toLowerCase();
        const roleFilter = $('#roleFilter').val();
        const statusFilter = $('#statusFilter').val();

        $('#usersTable tbody tr').each(function () {
            const row = $(this);
            const name = row.find('.user-name').text().toLowerCase();
            const email = row.find('td:eq(2)').text().toLowerCase();
            const role = row.find('.role-badge').text().trim();
            const status = row.find('.status-badge').hasClass('status-active') ? 'true' : 'false';

            let showRow = true;

            // Search filter
            if (searchTerm && !name.includes(searchTerm) && !email.includes(searchTerm)) {
                showRow = false;
            }

            // Role filter
            if (roleFilter && !role.includes(roleFilter === 'Student' ? 'Sinh Viên' : roleFilter === 'Teacher' ? 'Giáo Viên' : 'Quản Trị')) {
                showRow = false;
            }

            // Status filter
            if (statusFilter && status !== statusFilter) {
                showRow = false;
            }

            row.toggle(showRow);
        });

        updateTableInfo();
    }

    // Update table info
    function updateTableInfo() {
        const visibleRows = $('#usersTable tbody tr:visible').length;
        const totalRows = $('#usersTable tbody tr').length;
        $('#tableInfo').text(`Hiển thị ${visibleRows} / ${totalRows} người dùng`);
    }

    // Export users
    $('#exportUsers').on('click', function () {
        // Simple CSV export
        let csv = 'ID,Họ và Tên,Email,Vai Trò,Trạng Thái,Ngày Tạo,Đăng Nhập Cuối\n';
        
        $('#usersTable tbody tr:visible').each(function () {
            const row = $(this);
            const id = row.data('user-id');
            const name = row.find('.user-name').text();
            const email = row.find('td:eq(2)').text();
            const role = row.find('.role-badge').text().trim();
            const status = row.find('.status-badge').text().trim();
            const created = row.find('td:eq(5)').text();
            const lastLogin = row.find('td:eq(6)').text();
            
            csv += `${id},"${name}","${email}","${role}","${status}","${created}","${lastLogin}"\n`;
        });

        // Download CSV
        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', `users_${new Date().toISOString().split('T')[0]}.csv`);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        showNotification('success', 'Đã xuất danh sách người dùng thành công!');
    });

    // Show notification function
    function showNotification(type, message) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
        
        const notification = $(`
            <div class="alert ${alertClass} alert-dismissible fade show notification-toast" role="alert" style="position: fixed; top: 80px; right: 20px; z-index: 9999; min-width: 300px;">
                <i class="fas ${icon}"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `);

        $('body').append(notification);

        // Auto dismiss after 5 seconds
        setTimeout(function () {
            notification.fadeOut(400, function() {
                $(this).remove();
            });
        }, 5000);
    }

    // Initialize table info
    updateTableInfo();
});
