// CreateExam.js - Multi-step form wizard functionality

let currentStep = 1;
const totalSteps = 4;
let manualQuestionIndex = 0;
let manualQuestions = [];

document.addEventListener('DOMContentLoaded', function() {
    initializeForm();
    setupEventListeners();
    setupTabSystem();
    setupManualQuestions();
    updateStepDisplay();
});

// Initialize form
function initializeForm() {
    // Set default dates
    const now = new Date();
    const tomorrow = new Date(now);
    tomorrow.setDate(tomorrow.getDate() + 1);
    
    const oneWeekLater = new Date(now);
    oneWeekLater.setDate(oneWeekLater.getDate() + 7);
    
    document.getElementById('startDate').value = formatDateTimeLocal(tomorrow);
    document.getElementById('endDate').value = formatDateTimeLocal(oneWeekLater);
}

// Setup event listeners
function setupEventListeners() {
    // Navigation buttons
    document.getElementById('nextBtn').addEventListener('click', nextStep);
    document.getElementById('prevBtn').addEventListener('click', prevStep);
    document.getElementById('saveDraftBtn').addEventListener('click', saveDraft);
    document.getElementById('publishBtn').addEventListener('click', publishExam);
    
    // Form submission
    document.getElementById('createExamForm').addEventListener('submit', function(e) {
        e.preventDefault();
        publishExam();
    });
    
    // Bank question selection
    const questionCheckboxes = document.querySelectorAll('.question-checkbox');
    questionCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', updateQuestionCounts);
    });
    
    // Question item click (toggle checkbox)
    const questionItems = document.querySelectorAll('.question-item');
    questionItems.forEach(item => {
        item.addEventListener('click', function(e) {
            if (e.target.type !== 'checkbox') {
                const checkbox = this.querySelector('.question-checkbox');
                checkbox.checked = !checkbox.checked;
                this.classList.toggle('selected', checkbox.checked);
                updateSelectedQuestions();
            } else {
                this.classList.toggle('selected', e.target.checked);
            }
        });
    });
    
    // Question filters
    const filterCheckboxes = document.querySelectorAll('.question-filter');
    filterCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', filterQuestions);
    });
    
    // Real-time validation
    const requiredInputs = document.querySelectorAll('[required]');
    requiredInputs.forEach(input => {
        input.addEventListener('blur', function() {
            validateInput(this);
        });
        input.addEventListener('input', function() {
            if (this.classList.contains('error')) {
                validateInput(this);
            }
        });
    });
}

// Navigate to next step
function nextStep() {
    if (validateStep(currentStep)) {
        if (currentStep < totalSteps) {
            currentStep++;
            updateStepDisplay();
            
            // Update preview on last step
            if (currentStep === 4) {
                updatePreview();
            }
        }
    }
}

// Navigate to previous step
function prevStep() {
    if (currentStep > 1) {
        currentStep--;
        updateStepDisplay();
    }
}

// Update step display
function updateStepDisplay() {
    // Update step indicators
    const steps = document.querySelectorAll('.exam-step');
    steps.forEach((step, index) => {
        const stepNumber = index + 1;
        step.classList.remove('active', 'completed');
        
        if (stepNumber < currentStep) {
            step.classList.add('completed');
            step.querySelector('.exam-step-circle').innerHTML = '<i class="fas fa-check"></i>';
        } else if (stepNumber === currentStep) {
            step.classList.add('active');
            step.querySelector('.exam-step-circle').textContent = stepNumber;
        } else {
            step.querySelector('.exam-step-circle').textContent = stepNumber;
        }
    });
    
    // Update form sections
    const sections = document.querySelectorAll('.exam-form-section');
    sections.forEach((section, index) => {
        section.classList.toggle('active', index + 1 === currentStep);
    });
    
    // Update navigation buttons
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const publishBtn = document.getElementById('publishBtn');
    
    prevBtn.style.display = currentStep === 1 ? 'none' : 'inline-flex';
    nextBtn.style.display = currentStep === totalSteps ? 'none' : 'inline-flex';
    publishBtn.style.display = currentStep === totalSteps ? 'inline-flex' : 'none';
    
    // Scroll to top
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// Validate current step
function validateStep(step) {
    let isValid = true;
    
    switch(step) {
        case 1:
            // Validate basic information
            const examName = document.getElementById('examName');
            const subject = document.getElementById('subject');
            const duration = document.getElementById('duration');
            const startDate = document.getElementById('startDate');
            const endDate = document.getElementById('endDate');
            
            isValid = validateInput(examName) && 
                     validateInput(subject) && 
                     validateInput(duration) && 
                     validateInput(startDate) && 
                     validateInput(endDate);
            
            // Check if end date is after start date
            if (isValid && new Date(endDate.value) <= new Date(startDate.value)) {
                endDate.classList.add('error');
                const errorMsg = endDate.nextElementSibling;
                if (errorMsg && errorMsg.classList.contains('exam-error-message')) {
                    errorMsg.textContent = 'Ngày đóng đề phải sau ngày mở đề';
                    errorMsg.style.display = 'block';
                }
                isValid = false;
            }
            break;
            
        case 2:
            // Validate question selection (manual + bank)
            const manualCards = document.querySelectorAll('.manual-question-card');
            const bankQuestions = document.querySelectorAll('.question-checkbox:checked');
            
            if (manualCards.length === 0 && bankQuestions.length === 0) {
                alert('Vui lòng nhập câu hỏi thủ công hoặc chọn từ ngân hàng câu hỏi');
                isValid = false;
                break;
            }
            
            // Validate manual questions
            manualCards.forEach(card => {
                const questionText = card.querySelector('.manual-question-text').value.trim();
                const questionType = card.querySelector('.manual-question-type').value;
                const points = card.querySelector('.manual-question-points').value;
                
                if (!questionText) {
                    alert('Vui lòng nhập nội dung cho tất cả câu hỏi');
                    isValid = false;
                    return;
                }
                
                if (!points || points < 1) {
                    alert('Vui lòng nhập điểm hợp lệ cho tất cả câu hỏi (tối thiểu 1 điểm)');
                    isValid = false;
                    return;
                }
                
                // Validate answers for multiple choice questions
                if (questionType !== 'essay') {
                    const answers = card.querySelectorAll('.answer-text');
                    const checkedAnswer = card.querySelector('.answer-radio:checked');
                    
                    if (answers.length < 2) {
                        alert('Mỗi câu hỏi trắc nghiệm cần ít nhất 2 đáp án');
                        isValid = false;
                        return;
                    }
                    
                    // Check if all answers have text
                    let allAnswersFilled = true;
                    answers.forEach(answer => {
                        if (!answer.value.trim()) {
                            allAnswersFilled = false;
                        }
                    });
                    
                    if (!allAnswersFilled) {
                        alert('Vui lòng nhập nội dung cho tất cả đáp án');
                        isValid = false;
                        return;
                    }
                    
                    if (!checkedAnswer) {
                        alert('Vui lòng chọn đáp án đúng cho tất cả câu hỏi trắc nghiệm');
                        isValid = false;
                        return;
                    }
                }
            });
            break;
            
        case 3:
            // Validate configuration
            const assignTo = document.getElementById('assignTo');
            isValid = validateInput(assignTo);
            break;
    }
    
    return isValid;
}

// Validate individual input
function validateInput(input) {
    const value = input.value.trim();
    let isValid = true;
    
    // Clear previous error
    input.classList.remove('error');
    const errorMsg = input.nextElementSibling;
    if (errorMsg && errorMsg.classList.contains('exam-error-message')) {
        errorMsg.style.display = 'none';
    }
    
    // Check if required
    if (input.hasAttribute('required') && !value) {
        isValid = false;
    }
    
    // Check specific validations
    if (input.type === 'number' && value) {
        const min = parseInt(input.min);
        const max = parseInt(input.max);
        const numValue = parseInt(value);
        
        if ((min && numValue < min) || (max && numValue > max)) {
            isValid = false;
        }
    }
    
    // Show error if invalid
    if (!isValid) {
        input.classList.add('error');
        if (errorMsg && errorMsg.classList.contains('exam-error-message')) {
            errorMsg.style.display = 'block';
        }
    }
    
    return isValid;
}

// Update selected questions count
function updateSelectedQuestions() {
    const selectedCheckboxes = document.querySelectorAll('.question-checkbox:checked');
    const count = selectedCheckboxes.length;
    
    // Calculate total points
    let totalPoints = 0;
    selectedCheckboxes.forEach(checkbox => {
        const questionItem = checkbox.closest('.question-item');
        const pointsText = questionItem.querySelector('.question-meta').textContent;
        const points = parseInt(pointsText.match(/(\d+)\s+điểm/)?.[1] || 0);
        totalPoints += points;
    });
    
    document.getElementById('selectedCount').textContent = count;
    document.getElementById('totalPoints').textContent = totalPoints;
}

// Filter questions by difficulty
function filterQuestions() {
    const filters = document.querySelectorAll('.question-filter:checked');
    const difficulties = Array.from(filters).map(f => f.dataset.difficulty);
    
    // Handle "all" checkbox
    const allCheckbox = document.querySelector('.question-filter[data-difficulty="all"]');
    if (allCheckbox.checked) {
        // Uncheck other filters
        filters.forEach(f => {
            if (f.dataset.difficulty !== 'all') {
                f.checked = false;
            }
        });
    } else if (filters.length > 1) {
        // If other filters are checked, uncheck "all"
        allCheckbox.checked = false;
    }
    
    // Show/hide questions
    const questionItems = document.querySelectorAll('.question-item');
    questionItems.forEach(item => {
        const difficulty = item.dataset.difficulty;
        const shouldShow = difficulties.includes('all') || difficulties.includes(difficulty);
        item.style.display = shouldShow ? 'flex' : 'none';
    });
}

// Update preview
function updatePreview() {
    // Basic information
    const examName = document.getElementById('examName').value;
    const subject = document.getElementById('subject');
    const subjectText = subject.options[subject.selectedIndex].text;
    const duration = document.getElementById('duration').value;
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;
    
    document.getElementById('preview-examName').textContent = examName || '-';
    document.getElementById('preview-subject').textContent = subjectText || '-';
    document.getElementById('preview-duration').textContent = duration ? `${duration} phút` : '-';
    document.getElementById('preview-startDate').textContent = formatDateTime(startDate);
    document.getElementById('preview-endDate').textContent = formatDateTime(endDate);
    
    // Questions
    const manualCount = document.querySelectorAll('.manual-question-card').length;
    const bankCount = document.querySelectorAll('.question-checkbox:checked').length;
    const totalQuestions = manualCount + bankCount;
    const totalPoints = document.getElementById('totalPoints').textContent;
    
    document.getElementById('preview-questionCount').textContent = `${totalQuestions} (${manualCount} thủ công + ${bankCount} từ ngân hàng)`;
    document.getElementById('preview-totalPoints').textContent = totalPoints;
    
    // Configuration
    const shuffleQuestions = document.getElementById('shuffleQuestions').checked;
    const showResults = document.getElementById('showResults').checked;
    const assignTo = document.getElementById('assignTo');
    const assignToText = Array.from(assignTo.selectedOptions).map(opt => opt.text).join(', ');
    
    document.getElementById('preview-shuffle').textContent = shuffleQuestions ? 'Có' : 'Không';
    document.getElementById('preview-showResults').textContent = showResults ? 'Có' : 'Không';
    document.getElementById('preview-assignTo').textContent = assignToText || '-';
}

// Save as draft
function saveDraft() {
    if (!confirm('Bạn muốn lưu đề thi này dưới dạng nháp?')) {
        return;
    }
    
    showLoading(true);
    
    const formData = collectFormData();
    formData.status = 'draft';
    
    // Calculate total questions and points
    const manualCount = formData.manualQuestions.length;
    const bankCount = formData.bankQuestions.length;
    const totalQuestions = manualCount + bankCount;
    
    // Calculate total points (assuming 1 point per question for demo)
    let totalPoints = manualCount; // Manual questions = 1 point each
    formData.bankQuestions.forEach(() => {
        totalPoints += 1; // Bank questions = 1 point each (demo)
    });
    
    // Simulate API call
    setTimeout(() => {
        showLoading(false);
        
        // Redirect to success page with exam details
        const examName = encodeURIComponent(formData.examName);
        const status = 'draft';
        window.location.href = `/Teacher/CreateExamSuccess?examName=${examName}&status=${status}&questionCount=${totalQuestions}&totalPoints=${totalPoints}`;
    }, 1500);
}

// Publish exam
function publishExam() {
    if (!confirm('Bạn có chắc chắn muốn xuất bản đề thi này? Đề thi sẽ hiển thị cho sinh viên theo thời gian đã cấu hình.')) {
        return;
    }
    
    if (!validateStep(1) || !validateStep(2) || !validateStep(3)) {
        alert('Vui lòng kiểm tra lại thông tin các bước trước đó');
        return;
    }
    
    showLoading(true);
    
    const formData = collectFormData();
    formData.status = 'published';
    
    // Calculate total questions and points
    const manualCount = formData.manualQuestions.length;
    const bankCount = formData.bankQuestions.length;
    const totalQuestions = manualCount + bankCount;
    
    // Calculate total points (assuming 1 point per question for demo)
    let totalPoints = manualCount; // Manual questions = 1 point each
    formData.bankQuestions.forEach(() => {
        totalPoints += 1; // Bank questions = 1 point each (demo)
    });
    
    // Simulate API call
    setTimeout(() => {
        showLoading(false);
        
        // Redirect to success page with exam details
        const examName = encodeURIComponent(formData.examName);
        const status = 'published';
        window.location.href = `/Teacher/CreateExamSuccess?examName=${examName}&status=${status}&questionCount=${totalQuestions}&totalPoints=${totalPoints}`;
    }, 1500);
}

// Collect form data
function collectFormData() {
    // Collect manual questions
    const manualQuestions = [];
    document.querySelectorAll('.manual-question-card').forEach(card => {
        const questionData = {
            text: card.querySelector('.manual-question-text').value,
            type: card.querySelector('.manual-question-type').value,
            difficulty: card.querySelector('.manual-question-difficulty').value,
            points: card.querySelector('.manual-question-points').value,
            answers: []
        };
        
        // Collect answers if not essay
        if (questionData.type !== 'essay') {
            card.querySelectorAll('.answer-item').forEach(answerItem => {
                const answerText = answerItem.querySelector('.answer-text').value;
                const isCorrect = answerItem.querySelector('.answer-radio').checked;
                
                questionData.answers.push({
                    text: answerText,
                    isCorrect: isCorrect
                });
            });
        }
        
        manualQuestions.push(questionData);
    });
    
    const formData = {
        examName: document.getElementById('examName').value,
        subject: document.getElementById('subject').value,
        description: document.getElementById('description').value,
        duration: document.getElementById('duration').value,
        startDate: document.getElementById('startDate').value,
        endDate: document.getElementById('endDate').value,
        manualQuestions: manualQuestions,
        bankQuestions: Array.from(document.querySelectorAll('.question-checkbox:checked')).map(cb => cb.value),
        shuffleQuestions: document.getElementById('shuffleQuestions').checked,
        shuffleAnswers: document.getElementById('shuffleAnswers').checked,
        showResults: document.getElementById('showResults').checked,
        allowReview: document.getElementById('allowReview').checked,
        assignTo: Array.from(document.getElementById('assignTo').selectedOptions).map(opt => opt.value),
        passingScore: document.getElementById('passingScore').value
    };
    
    console.log('Form Data:', formData); // Debug
    return formData;
}

// Show/hide loading spinner
function showLoading(show) {
    const loadingSpinner = document.getElementById('loadingSpinner');
    const formContainer = document.querySelector('.exam-form-container');
    
    if (show) {
        loadingSpinner.classList.add('active');
        formContainer.style.display = 'none';
    } else {
        loadingSpinner.classList.remove('active');
        formContainer.style.display = 'block';
    }
}

// Helper: Format datetime for input
function formatDateTimeLocal(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    
    return `${year}-${month}-${day}T${hours}:${minutes}`;
}

// Helper: Format datetime for display
function formatDateTime(dateString) {
    if (!dateString) return '-';
    
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    
    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

// ============ TAB SYSTEM ============
function setupTabSystem() {
    const tabs = document.querySelectorAll('.question-tab');
    const panels = document.querySelectorAll('.question-tab-panel');
    
    tabs.forEach(tab => {
        tab.addEventListener('click', function() {
            const targetPanel = this.dataset.tab;
            
            // Update active tab
            tabs.forEach(t => t.classList.remove('active'));
            this.classList.add('active');
            
            // Update active panel
            panels.forEach(p => {
                p.classList.toggle('active', p.dataset.panel === targetPanel);
            });
        });
    });
}

// ============ MANUAL QUESTIONS ============
function setupManualQuestions() {
    const addBtn = document.getElementById('addManualQuestionBtn');
    if (addBtn) {
        addBtn.addEventListener('click', addManualQuestion);
    }
}

function addManualQuestion() {
    manualQuestionIndex++;
    const template = document.getElementById('manualQuestionTemplate');
    const clone = template.content.cloneNode(true);
    
    // Set question number
    const card = clone.querySelector('.manual-question-card');
    card.dataset.questionIndex = manualQuestionIndex;
    clone.querySelector('.question-number').textContent = manualQuestionIndex;
    
    // Setup delete button
    const deleteBtn = clone.querySelector('.delete-question-btn');
    deleteBtn.addEventListener('click', function() {
        deleteManualQuestion(this.closest('.manual-question-card'));
    });
    
    // Setup question type change
    const typeSelect = clone.querySelector('.manual-question-type');
    typeSelect.addEventListener('change', function() {
        handleQuestionTypeChange(this);
    });
    
    // Setup add answer button
    const addAnswerBtn = clone.querySelector('.add-answer-btn');
    addAnswerBtn.addEventListener('click', function() {
        addAnswer(this.closest('.manual-question-card'));
    });
    
    // Setup points input change
    const pointsInput = clone.querySelector('.manual-question-points');
    pointsInput.addEventListener('change', updateQuestionCounts);
    
    // Add 4 default answers for multiple choice
    const answersList = clone.querySelector('.answers-list');
    for (let i = 0; i < 4; i++) {
        const answerClone = createAnswerElement(manualQuestionIndex);
        answersList.appendChild(answerClone);
    }
    
    // Append to list
    document.getElementById('manualQuestionsList').appendChild(clone);
    
    // Update counts
    updateQuestionCounts();
    
    // Scroll to new question
    setTimeout(() => {
        card.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }, 100);
}

function createAnswerElement(questionIndex) {
    const template = document.getElementById('answerTemplate');
    const clone = template.content.cloneNode(true);
    
    // Update radio button name to be unique per question
    const radio = clone.querySelector('.answer-radio');
    radio.name = `correct-answer-${questionIndex}`;
    
    // Setup delete button
    const deleteBtn = clone.querySelector('.delete-answer-btn');
    deleteBtn.addEventListener('click', function() {
        this.closest('.answer-item').remove();
    });
    
    return clone;
}

function addAnswer(questionCard) {
    const questionIndex = questionCard.dataset.questionIndex;
    const answersList = questionCard.querySelector('.answers-list');
    const answerElement = createAnswerElement(questionIndex);
    answersList.appendChild(answerElement);
}

function deleteManualQuestion(questionCard) {
    if (!confirm('Bạn có chắc chắn muốn xóa câu hỏi này?')) {
        return;
    }
    
    questionCard.remove();
    updateQuestionCounts();
    renumberQuestions();
}

function renumberQuestions() {
    const questions = document.querySelectorAll('.manual-question-card');
    questions.forEach((question, index) => {
        question.querySelector('.question-number').textContent = index + 1;
    });
}

function handleQuestionTypeChange(select) {
    const card = select.closest('.manual-question-card');
    const answersSection = card.querySelector('.answers-section');
    const type = select.value;
    
    // Show/hide answers section based on question type
    if (type === 'essay') {
        answersSection.style.display = 'none';
    } else if (type === 'true-false') {
        // Clear existing answers and add True/False
        const answersList = answersSection.querySelector('.answers-list');
        answersList.innerHTML = '';
        
        const questionIndex = card.dataset.questionIndex;
        
        // Add True answer
        const trueAnswer = createAnswerElement(questionIndex);
        trueAnswer.querySelector('.answer-text').value = 'Đúng';
        trueAnswer.querySelector('.answer-text').readOnly = true;
        trueAnswer.querySelector('.delete-answer-btn').style.display = 'none';
        answersList.appendChild(trueAnswer);
        
        // Add False answer
        const falseAnswer = createAnswerElement(questionIndex);
        falseAnswer.querySelector('.answer-text').value = 'Sai';
        falseAnswer.querySelector('.answer-text').readOnly = true;
        falseAnswer.querySelector('.delete-answer-btn').style.display = 'none';
        answersList.appendChild(falseAnswer);
        
        answersSection.style.display = 'block';
        card.querySelector('.add-answer-btn').style.display = 'none';
    } else {
        answersSection.style.display = 'block';
        card.querySelector('.add-answer-btn').style.display = 'block';
    }
}

// Update question counts (both manual and bank)
function updateQuestionCounts() {
    // Count manual questions
    const manualCards = document.querySelectorAll('.manual-question-card');
    let manualCount = manualCards.length;
    let manualPoints = 0;
    
    manualCards.forEach(card => {
        const points = parseInt(card.querySelector('.manual-question-points')?.value || 0);
        manualPoints += points;
    });
    
    // Count bank questions
    const bankCheckboxes = document.querySelectorAll('.question-checkbox:checked');
    let bankCount = bankCheckboxes.length;
    let bankPoints = 0;
    
    bankCheckboxes.forEach(checkbox => {
        const item = checkbox.closest('.question-item');
        const points = parseInt(item?.dataset.points || 0);
        bankPoints += points;
    });
    
    // Update UI
    document.getElementById('manualCount').textContent = manualCount;
    document.getElementById('bankCount').textContent = bankCount;
    document.getElementById('totalPoints').textContent = manualPoints + bankPoints;
}

// Update old function name
function updateSelectedQuestions() {
    updateQuestionCounts();
}
