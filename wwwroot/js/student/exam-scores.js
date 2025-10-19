/* ===================================
   EXAM SCORES PAGE JAVASCRIPT
   E_TestHub v0.3.0 - Student Module
   =================================== */

// Filter exams by search, status, and subject
function filterExams() {
    const searchInput = document.getElementById('searchInput').value.toLowerCase();
    const statusFilter = document.getElementById('statusFilter').value;
    const subjectFilter = document.getElementById('subjectFilter').value;
    
    const rows = document.querySelectorAll('.score-row');
    let visibleCount = 0;
    
    rows.forEach(row => {
        const examName = row.querySelector('.exam-name span').textContent.toLowerCase();
        const subject = row.dataset.subject;
        const status = row.dataset.status;
        
        // Check all filters
        const matchesSearch = examName.includes(searchInput) || subject.toLowerCase().includes(searchInput);
        const matchesStatus = statusFilter === 'all' || status === statusFilter;
        const matchesSubject = subjectFilter === 'all' || subject === subjectFilter;
        
        if (matchesSearch && matchesStatus && matchesSubject) {
            row.style.display = '';
            visibleCount++;
        } else {
            row.style.display = 'none';
        }
    });
    
    // Show/hide empty state
    const emptyState = document.getElementById('emptyState');
    const tableContainer = document.querySelector('.scores-table-container');
    
    if (visibleCount === 0) {
        emptyState.style.display = 'block';
        tableContainer.style.display = 'none';
    } else {
        emptyState.style.display = 'none';
        tableContainer.style.display = 'block';
    }
}

// Sort exams by date or score
function sortExams() {
    const sortBy = document.getElementById('sortBy').value;
    const tbody = document.querySelector('.scores-table tbody');
    const rows = Array.from(tbody.querySelectorAll('.score-row'));
    
    rows.sort((a, b) => {
        switch(sortBy) {
            case 'date-desc':
                return parseInt(b.dataset.date) - parseInt(a.dataset.date);
            case 'date-asc':
                return parseInt(a.dataset.date) - parseInt(b.dataset.date);
            case 'score-desc':
                return parseFloat(b.dataset.score) - parseFloat(a.dataset.score);
            case 'score-asc':
                return parseFloat(a.dataset.score) - parseFloat(b.dataset.score);
            default:
                return 0;
        }
    });
    
    // Re-append rows in sorted order
    rows.forEach(row => tbody.appendChild(row));
}

// Export scores to CSV (demo)
function exportScores() {
    const rows = document.querySelectorAll('.score-row');
    let csvContent = 'Tên bài thi,Môn học,Ngày thi,Điểm số,Trạng thái\n';
    
    rows.forEach(row => {
        if (row.style.display !== 'none') {
            const examName = row.querySelector('.exam-name span').textContent;
            const subject = row.querySelector('.subject-badge').textContent;
            const date = row.querySelector('.exam-date').textContent.trim();
            const scoreValue = row.querySelector('.score-value');
            const score = scoreValue ? scoreValue.textContent : '-';
            const status = row.querySelector('.status-badge').textContent.trim();
            
            csvContent += `"${examName}","${subject}","${date}","${score}","${status}"\n`;
        }
    });
    
    // Create download link
    const blob = new Blob(['\uFEFF' + csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', 'diem_thi_' + new Date().getTime() + '.csv');
    link.style.visibility = 'hidden';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    // Show success message
    showNotification('Đã xuất báo cáo thành công!', 'success');
}

// Show notification (simple toast)
function showNotification(message, type) {
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 16px 24px;
        background: ${type === 'success' ? '#10B981' : '#EF4444'};
        color: white;
        border-radius: 10px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        font-family: 'Inter', sans-serif;
        font-size: 15px;
        font-weight: 600;
        z-index: 9999;
        animation: slideInRight 0.3s ease-out;
    `;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.style.animation = 'slideOutRight 0.3s ease-out';
        setTimeout(() => {
            document.body.removeChild(notification);
        }, 300);
    }, 3000);
}

// Add keyframe animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    console.log('Exam Scores page loaded');
    
    // Set default sort (newest first)
    sortExams();
});
