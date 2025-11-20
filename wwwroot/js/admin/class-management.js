// Class Management JavaScript
$(document).ready(function() {
    // Initialize
    initializeFilters();
    initializeSearch();
    initializeDeleteHandlers();
    populateAcademicYearFilter();

    // Search functionality
    function initializeSearch() {
        $('#searchInput').on('keyup', function() {
            const searchTerm = $(this).val().toLowerCase();
            const selectedYear = $('#academicYearFilter').val();
            
            filterTable(searchTerm, selectedYear);
        });
    }

    // Academic year filter
    function initializeFilters() {
        $('#academicYearFilter').on('change', function() {
            const selectedYear = $(this).val();
            const searchTerm = $('#searchInput').val().toLowerCase();
            
            filterTable(searchTerm, selectedYear);
        });
    }

    // Populate academic year dropdown from table data
    function populateAcademicYearFilter() {
        const years = new Set();
        
        $('#classTable tbody tr').each(function() {
            const year = $(this).data('academic-year');
            if (year) {
                years.add(year);
            }
        });

        const sortedYears = Array.from(years).sort().reverse();
        
        sortedYears.forEach(year => {
            $('#academicYearFilter').append(
                `<option value="${year}">${year}</option>`
            );
        });
    }

    // Filter table based on search and filter
    function filterTable(searchTerm, academicYear) {
        $('#classTable tbody tr').each(function() {
            const row = $(this);
            const text = row.text().toLowerCase();
            const rowYear = row.data('academic-year');
            
            const matchesSearch = text.includes(searchTerm);
            const matchesYear = !academicYear || rowYear === academicYear;
            
            row.toggle(matchesSearch && matchesYear);
        });

        updateNoResultsMessage();
    }

    // Show "no results" message if table is empty
    function updateNoResultsMessage() {
        const visibleRows = $('#classTable tbody tr:visible').length;
        
        if (visibleRows === 0) {
            if ($('#noResultsRow').length === 0) {
                $('#classTable tbody').append(`
                    <tr id="noResultsRow">
                        <td colspan="8" class="text-center py-4">
                            <i class="fas fa-search fa-2x text-muted mb-2"></i>
                            <p class="text-muted mb-0">Không tìm thấy kết quả phù hợp</p>
                        </td>
                    </tr>
                `);
            }
        } else {
            $('#noResultsRow').remove();
        }
    }

    // Delete class handler
    function initializeDeleteHandlers() {
        $('.delete-class').on('click', function(e) {
            e.preventDefault();
            
            const classId = $(this).data('id');
            const className = $(this).data('name');
            const row = $(this).closest('tr');
            
            if (!confirm(`Bạn có chắc chắn muốn xóa lớp học "${className}"?\n\nLưu ý: Thao tác này không thể hoàn tác!`)) {
                return;
            }

            const token = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: '/Class/Delete',
                type: 'POST',
                data: {
                    __RequestVerificationToken: token,
                    id: classId
                },
                success: function(response) {
                    if (response.success) {
                        row.fadeOut(300, function() {
                            $(this).remove();
                            updateStatistics();
                            showNotification('success', response.message);
                        });
                    } else {
                        showNotification('error', response.message);
                    }
                },
                error: function(xhr) {
                    let errorMessage = 'Có lỗi xảy ra khi xóa lớp học';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    showNotification('error', errorMessage);
                }
            });
        });
    }

    // Update statistics after delete
    function updateStatistics() {
        const totalClasses = $('#classTable tbody tr:visible').length;
        const totalStudents = calculateTotalStudents();
        
        // Update cards
        $('.bg-primary .card-body h3').text(totalClasses);
        $('.bg-info .card-body h3').text(totalStudents);

        // Show empty state if no classes
        if (totalClasses === 0) {
            $('.table-responsive').html(`
                <div class="text-center py-5">
                    <i class="fas fa-chalkboard-teacher fa-4x text-muted mb-3"></i>
                    <h5 class="text-muted">Chưa có lớp học nào</h5>
                    <p class="text-muted mb-3">Hãy tạo lớp học đầu tiên để bắt đầu</p>
                    <a href="/Class/Create" class="btn btn-primary">
                        <i class="fas fa-plus me-2"></i>Tạo Lớp học
                    </a>
                </div>
            `);
        }
    }

    // Calculate total students across all classes
    function calculateTotalStudents() {
        let total = 0;
        $('#classTable tbody tr:visible').each(function() {
            const studentCount = parseInt($(this).find('.badge.bg-success').text()) || 0;
            total += studentCount;
        });
        return total;
    }

    // Export to CSV
    window.exportToCSV = function() {
        const rows = [];
        const headers = ['STT', 'Mã Lớp', 'Tên Lớp', 'Giảng viên', 'Năm học', 'Sĩ số', 'Ngày tạo'];
        rows.push(headers);

        $('#classTable tbody tr:visible').each(function(index) {
            const row = [];
            row.push(index + 1);
            row.push($(this).find('td:eq(1)').text().trim());
            row.push($(this).find('td:eq(2)').text().trim());
            row.push($(this).find('td:eq(3)').text().trim());
            row.push($(this).find('td:eq(4)').text().trim());
            row.push($(this).find('td:eq(5)').text().trim());
            row.push($(this).find('td:eq(6)').text().trim());
            rows.push(row);
        });

        const csvContent = rows.map(row => row.join(',')).join('\n');
        const blob = new Blob(['\ufeff' + csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        
        link.setAttribute('href', url);
        link.setAttribute('download', `DanhSachLopHoc_${new Date().getTime()}.csv`);
        link.style.visibility = 'hidden';
        
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        showNotification('success', 'Xuất file Excel thành công!');
    };

    // Show notification
    function showNotification(type, message) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        const iconClass = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
        
        const alert = $(`
            <div class="alert ${alertClass} alert-dismissible fade show position-fixed" 
                 role="alert" 
                 style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
                <i class="fas ${iconClass} me-2"></i>${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `);
        
        $('body').append(alert);
        
        setTimeout(function() {
            alert.alert('close');
        }, 3000);
    }

    // Bulk operations (for future enhancement)
    let selectedClasses = [];

    $('#selectAll').on('change', function() {
        const isChecked = $(this).is(':checked');
        $('.class-checkbox').prop('checked', isChecked);
        updateSelectedClasses();
    });

    $('.class-checkbox').on('change', function() {
        updateSelectedClasses();
    });

    function updateSelectedClasses() {
        selectedClasses = [];
        $('.class-checkbox:checked').each(function() {
            selectedClasses.push($(this).val());
        });

        // Show/hide bulk action buttons
        if (selectedClasses.length > 0) {
            $('#bulkActions').removeClass('d-none');
            $('#selectedCount').text(selectedClasses.length);
        } else {
            $('#bulkActions').addClass('d-none');
        }
    }

    // Bulk delete (for future enhancement)
    $('#bulkDelete').on('click', function() {
        if (selectedClasses.length === 0) {
            showNotification('error', 'Vui lòng chọn ít nhất một lớp học');
            return;
        }

        if (!confirm(`Bạn có chắc chắn muốn xóa ${selectedClasses.length} lớp học đã chọn?`)) {
            return;
        }

        // Implement bulk delete logic here
        showNotification('info', 'Tính năng xóa hàng loạt đang được phát triển');
    });
});
