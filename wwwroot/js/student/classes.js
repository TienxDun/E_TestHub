/**
 * Student Classes Page - JavaScript
 * File: classes.js
 * Description: Multiple view modes, search, sort, and filter functionality
 */

document.addEventListener('DOMContentLoaded', function() {
    // Get DOM elements
    const searchInput = document.getElementById('searchInput');
    const sortSelect = document.getElementById('sortSelect');
    const classesGrid = document.getElementById('classesGrid');
    const classesList = document.getElementById('classesList');
    const classesTableContainer = document.getElementById('classesTableContainer');
    const classesTableBody = document.getElementById('classesTableBody');
    const emptyState = document.getElementById('emptyState');
    const classCards = document.querySelectorAll('.class-card');
    const viewBtns = document.querySelectorAll('.view-btn');
    
    // State management
    let currentView = localStorage.getItem('classesViewMode') || 'list';
    let classesData = [];

    // Extract data from grid cards
    extractClassesData();

    // Initialize view
    setActiveView(currentView);

    // View toggle event listeners
    viewBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const view = this.getAttribute('data-view');
            setActiveView(view);
            localStorage.setItem('classesViewMode', view);
        });
    });

    // Search functionality
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase().trim();
            filterAndDisplay();
        });
    }

    // Sort functionality
    if (sortSelect) {
        sortSelect.addEventListener('change', function() {
            filterAndDisplay();
        });
    }

    /**
     * Extract classes data from grid cards
     */
    function extractClassesData() {
        classCards.forEach(card => {
            const data = {
                name: card.getAttribute('data-class-name'),
                students: parseInt(card.getAttribute('data-students')),
                year: card.getAttribute('data-year')
            };
            classesData.push(data);
        });
    }

    /**
     * Set active view mode
     * @param {string} view - The view mode (grid, list, table)
     */
    function setActiveView(view) {
        currentView = view;

        // Update button states
        viewBtns.forEach(btn => {
            if (btn.getAttribute('data-view') === view) {
                btn.classList.add('active');
            } else {
                btn.classList.remove('active');
            }
        });

        // Hide all views
        classesGrid.style.display = 'none';
        classesList.style.display = 'none';
        classesTableContainer.style.display = 'none';

        // Display and populate the selected view
        filterAndDisplay();
    }

    /**
     * Filter and display classes based on search and sort
     */
    function filterAndDisplay() {
        const searchTerm = searchInput.value.toLowerCase().trim();
        const sortValue = sortSelect.value;

        // Filter classes
        let filteredClasses = classesData.filter(classItem => {
            return classItem.name.toLowerCase().includes(searchTerm) ||
                   classItem.students.toString().includes(searchTerm) ||
                   classItem.year.toLowerCase().includes(searchTerm);
        });

        // Sort classes
        filteredClasses = sortClasses(filteredClasses, sortValue);

        // Display based on current view
        if (filteredClasses.length === 0) {
            hideAllViews();
            emptyState.style.display = 'block';
        } else {
            emptyState.style.display = 'none';
            
            switch(currentView) {
                case 'grid':
                    displayGridView(filteredClasses);
                    break;
                case 'list':
                    displayListView(filteredClasses);
                    break;
                case 'table':
                    displayTableView(filteredClasses);
                    break;
            }
        }
    }

    /**
     * Sort classes based on selected criteria
     * @param {Array} classes - Array of class objects
     * @param {string} sortValue - Sort criteria
     * @returns {Array} Sorted array
     */
    function sortClasses(classes, sortValue) {
        const sorted = [...classes];
        
        switch(sortValue) {
            case 'name-asc':
                sorted.sort((a, b) => a.name.localeCompare(b.name));
                break;
            case 'name-desc':
                sorted.sort((a, b) => b.name.localeCompare(a.name));
                break;
            case 'students-asc':
                sorted.sort((a, b) => a.students - b.students);
                break;
            case 'students-desc':
                sorted.sort((a, b) => b.students - a.students);
                break;
        }
        
        return sorted;
    }

    /**
     * Hide all view containers
     */
    function hideAllViews() {
        classesGrid.style.display = 'none';
        classesList.style.display = 'none';
        classesTableContainer.style.display = 'none';
    }

    /**
     * Display grid view
     * @param {Array} classes - Filtered classes array
     */
    function displayGridView(classes) {
        classesGrid.style.display = 'grid';
        
        // Show/hide cards based on filtered data
        classCards.forEach(card => {
            const cardName = card.getAttribute('data-class-name');
            const shouldShow = classes.some(c => c.name === cardName);
            
            card.style.display = shouldShow ? 'block' : 'none';
            if (shouldShow) {
                card.style.animation = 'fadeIn 0.3s ease';
            }
        });
    }

    /**
     * Display list view
     * @param {Array} classes - Filtered classes array
     */
    function displayListView(classes) {
        classesList.style.display = 'flex';
        classesList.innerHTML = '';

        classes.forEach((classItem, index) => {
            const listItem = createListItem(classItem);
            listItem.style.opacity = '0';
            listItem.style.animation = `fadeInUp 0.5s ease ${index * 0.05}s forwards`;
            classesList.appendChild(listItem);
        });
    }

    /**
     * Create list item element
     * @param {Object} classItem - Class data object
     * @returns {HTMLElement} List item element
     */
    function createListItem(classItem) {
        const div = document.createElement('div');
        div.className = 'list-item';
        div.innerHTML = `
            <div class="list-item-content">
                <div class="class-icon">
                    <i class="fas fa-book-open"></i>
                </div>
                <div class="list-item-info">
                    <div class="list-item-column">
                        <h3>${classItem.name}</h3>
                        <p class="label">Mã lớp học</p>
                    </div>
                    <div class="list-item-column">
                        <p><strong>${classItem.students}</strong> sinh viên</p>
                        <p class="label">Sĩ số</p>
                    </div>
                    <div class="list-item-column">
                        <p>${classItem.year}</p>
                        <p class="label">Khoá học</p>
                    </div>
                </div>
                <div class="list-item-actions">
                    <a href="/Student/ClassDetails?classId=${classItem.name}" class="list-action-btn">
                        <i class="fas fa-eye"></i> Chi tiết
                    </a>
                </div>
            </div>
        `;
        return div;
    }

    /**
     * Display table view
     * @param {Array} classes - Filtered classes array
     */
    function displayTableView(classes) {
        classesTableContainer.style.display = 'block';
        classesTableBody.innerHTML = '';

        classes.forEach((classItem, index) => {
            const row = createTableRow(classItem);
            row.style.opacity = '0';
            row.style.animation = `fadeIn 0.3s ease ${index * 0.03}s forwards`;
            classesTableBody.appendChild(row);
        });
    }

    /**
     * Create table row element
     * @param {Object} classItem - Class data object
     * @returns {HTMLElement} Table row element
     */
    function createTableRow(classItem) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${classItem.name}</td>
            <td>${classItem.students} sinh viên</td>
            <td>${classItem.year}</td>
            <td>
                <div class="table-actions">
                    <a href="/Student/ClassDetails?classId=${classItem.name}" class="table-action-btn">
                        <i class="fas fa-eye"></i> Xem
                    </a>
                </div>
            </td>
        `;
        return tr;
    }

    /**
     * Handle class card click - Navigate to class detail
     * Note: Cards are now anchor tags, so click event is handled by href
     */
    // Removed click event listener as cards are now <a> tags with href

    /**
     * Add keyboard navigation support
     */
    searchInput.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
            this.value = '';
            filterAndDisplay();
            this.blur();
        }
    });

    // Table header sorting
    const tableSortHeaders = document.querySelectorAll('.classes-table th.sortable');
    tableSortHeaders.forEach(header => {
        header.addEventListener('click', function() {
            const sortType = this.getAttribute('data-sort');
            handleTableSort(sortType);
        });
    });

    /**
     * Handle table header sorting
     * @param {string} sortType - Sort type (name, students, year)
     */
    function handleTableSort(sortType) {
        const currentSort = sortSelect.value;
        let newSort = '';

        // Toggle sort direction
        if (currentSort.startsWith(sortType)) {
            newSort = currentSort.endsWith('asc') ? `${sortType}-desc` : `${sortType}-asc`;
        } else {
            newSort = `${sortType}-asc`;
        }

        sortSelect.value = newSort;
        filterAndDisplay();
    }
});

/**
 * Global function to view class detail
 * @param {string} className - The class name
 */
function viewClassDetail(className) {
    console.log(`Viewing class detail: ${className}`);
    
    // Navigate to class detail page
    window.location.href = `/Student/ClassDetails?classId=${className}`;
}

/**
 * Add CSS animations dynamically
 */
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeIn {
        from {
            opacity: 0;
        }
        to {
            opacity: 1;
        }
    }

    @keyframes fadeInUp {
        from {
            opacity: 0;
            transform: translateY(20px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
`;
document.head.appendChild(style);
