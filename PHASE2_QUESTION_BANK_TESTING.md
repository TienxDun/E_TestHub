# 🧪 Phase 2 - Question Bank Testing Checklist

## 📋 Testing Information
- **Date**: November 10, 2025
- **App URL**: http://localhost:5230
- **Test User**: Teacher account (role = teacher)
- **MongoDB API**: http://localhost:3000/api

---

## ✅ Pre-Test Setup

### 1. Ensure MongoDB API is running
```bash
# Navigate to e-testhub folder
cd d:\DevProject\CNPMNC\LyThuyet\New\e-testhub
npm start
```

### 2. Ensure you have test data
- At least 2-3 Subjects in MongoDB
- Login with a Teacher account

### 3. Login to Application
1. Navigate to: http://localhost:5230
2. Login with teacher credentials
3. Navigate to "Ngân hàng câu hỏi" from sidebar

---

## 🎯 Test Cases

### **TC1: View Question Bank (GET /Teacher/QuestionBank)**

**Prerequisites**: 
- Logged in as Teacher
- MongoDB API running

**Steps**:
1. Click "Ngân hàng câu hỏi" in sidebar
2. Page should load successfully

**Expected Results**:
- ✅ Page loads without errors
- ✅ Statistics cards display correct numbers:
  - Total questions
  - Total subjects
  - Easy questions count
  - Hard questions count
- ✅ Filter section visible with dropdowns for:
  - Subject
  - Difficulty (Dễ, Trung bình, Khó)
  - Question Type (Trắc nghiệm, Tự luận, Đúng/Sai)
- ✅ Questions table displays (if data exists)
- ✅ "Thêm câu hỏi" action card visible
- ✅ Import/Export cards visible (placeholder)

**Current Status**: ⏳ Pending Test

---

### **TC2: Create New Question - Multiple Choice (POST /Teacher/CreateQuestion)**

**Steps**:
1. Click "Thêm câu hỏi" button/card
2. Fill in form:
   - **Subject**: Select "Xác suất thống kê" (or any subject)
   - **Content**: "Xác suất của biến cố chắc chắn bằng bao nhiêu?"
   - **Type**: Trắc nghiệm (Multiple Choice)
   - **Option A**: "0"
   - **Option B**: "0.5"
   - **Option C**: "1"
   - **Option D**: "Không xác định"
   - **Correct Answer**: C
   - **Score**: 1
   - **Difficulty**: Dễ (Easy)
3. Click "Lưu câu hỏi"

**Expected Results**:
- ✅ Form validation passes
- ✅ Success message: "Câu hỏi đã được tạo thành công!"
- ✅ Redirects to Question Bank page
- ✅ New question appears in table
- ✅ Statistics updated (Total +1, Easy +1)
- ✅ Console logs show API POST request
- ✅ MongoDB contains new question

**Test Data**:
```json
{
  "subjectId": "[SUBJECT_ID_FROM_DROPDOWN]",
  "content": "Xác suất của biến cố chắc chắn bằng bao nhiêu?",
  "type": "multiplechoice",
  "options": ["0", "0.5", "1", "Không xác định"],
  "correctAnswer": "C",
  "score": 1,
  "difficultyLevel": "easy"
}
```

**Current Status**: ⏳ Pending Test

---

### **TC3: Create New Question - Essay Type**

**Steps**:
1. Click "Thêm câu hỏi"
2. Fill in form:
   - **Subject**: Select any subject
   - **Content**: "Phân tích các phương pháp ước lượng tham số trong thống kê"
   - **Type**: Tự luận (Essay)
   - **Score**: 5
   - **Difficulty**: Khó (Hard)
3. Click "Lưu câu hỏi"

**Expected Results**:
- ✅ Options section is hidden (no Options A-D)
- ✅ Correct Answer section is hidden
- ✅ Form submits successfully
- ✅ Success message displayed
- ✅ Question appears in Question Bank

**Current Status**: ⏳ Pending Test

---

### **TC4: Create New Question - True/False Type**

**Steps**:
1. Click "Thêm câu hỏi"
2. Fill in form:
   - **Subject**: Select any subject
   - **Content**: "Phân phối chuẩn có hai tham số: trung bình và phương sai"
   - **Type**: Đúng/Sai (True/False)
   - **Correct Answer**: Đúng (True)
   - **Score**: 1
   - **Difficulty**: Dễ (Easy)
3. Click "Lưu câu hỏi"

**Expected Results**:
- ✅ Options section is hidden
- ✅ Correct Answer dropdown shows only "Đúng" and "Sai"
- ✅ Form submits successfully
- ✅ Question appears in Question Bank

**Current Status**: ⏳ Pending Test

---

### **TC5: Form Validation - Create Question**

**Steps**:
1. Click "Thêm câu hỏi"
2. Leave all fields empty
3. Click "Lưu câu hỏi"

**Expected Results**:
- ❌ Form should NOT submit
- ✅ Validation error messages displayed:
  - "Môn học là bắt buộc"
  - "Nội dung câu hỏi là bắt buộc"
  - "Loại câu hỏi là bắt buộc"
  - "Điểm là bắt buộc"
- ✅ No API call made

**Current Status**: ⏳ Pending Test

---

### **TC6: Edit Existing Question (GET & POST /Teacher/EditQuestion)**

**Prerequisites**: At least one question exists

**Steps**:
1. In Question Bank table, click "Edit" (pencil icon) on any question
2. Edit Question page loads
3. Modify some fields:
   - Change Content
   - Change Score to 2
   - Change Difficulty to "Trung bình"
4. Click "Cập nhật câu hỏi"

**Expected Results**:
- ✅ Edit form loads with pre-filled data
- ✅ All fields populated correctly
- ✅ Subject dropdown shows current subject selected
- ✅ Question type shows correct type
- ✅ Options populated (if Multiple Choice)
- ✅ Correct answer pre-selected
- ✅ After submit: Success message "Câu hỏi đã được cập nhật thành công!"
- ✅ Redirects to Question Bank
- ✅ Updated question shows new values
- ✅ Console logs show API PUT request

**Current Status**: ⏳ Pending Test

---

### **TC7: Delete Question (POST /Teacher/DeleteQuestion via AJAX)**

**Prerequisites**: At least one question exists

**Steps**:
1. In Question Bank table, click "Delete" (trash icon) on any question
2. Confirm deletion in alert dialog

**Expected Results**:
- ✅ Confirmation dialog appears: "Bạn có chắc chắn muốn xóa câu hỏi này?"
- ✅ After confirmation, row fades out
- ✅ Page reloads
- ✅ Question removed from table
- ✅ Statistics updated (Total -1)
- ✅ Console logs show AJAX DELETE request
- ✅ MongoDB question deleted

**Current Status**: ⏳ Pending Test

---

### **TC8: Filter Questions by Subject**

**Prerequisites**: Questions exist for multiple subjects

**Steps**:
1. In Question Bank page
2. Select a specific subject from "Môn học" dropdown
3. Click "Lọc" button

**Expected Results**:
- ✅ Page reloads with filtered results
- ✅ Only questions for selected subject displayed
- ✅ URL contains: `?subjectId=[SUBJECT_ID]`
- ✅ Subject dropdown retains selected value
- ✅ Statistics reflect filtered data

**Current Status**: ⏳ Pending Test

---

### **TC9: Filter Questions by Difficulty**

**Steps**:
1. Select "Dễ" from "Độ khó" dropdown
2. Click "Lọc"

**Expected Results**:
- ✅ Only "Easy" questions displayed
- ✅ Badge color matches (green for Easy)
- ✅ URL contains: `?difficulty=Easy`
- ✅ Dropdown retains "Dễ" selection

**Current Status**: ⏳ Pending Test

---

### **TC10: Filter Questions by Type**

**Steps**:
1. Select "Trắc nghiệm" from "Loại câu hỏi" dropdown
2. Click "Lọc"

**Expected Results**:
- ✅ Only Multiple Choice questions displayed
- ✅ Type column shows "Trắc nghiệm"
- ✅ URL contains: `?type=MultipleChoice`

**Current Status**: ⏳ Pending Test

---

### **TC11: Combined Filters**

**Steps**:
1. Select Subject: "Xác suất thống kê"
2. Select Difficulty: "Trung bình"
3. Select Type: "Trắc nghiệm"
4. Click "Lọc"

**Expected Results**:
- ✅ Only questions matching ALL filters displayed
- ✅ URL contains all parameters: `?subjectId=X&difficulty=Medium&type=MultipleChoice`
- ✅ All dropdowns retain selections

**Current Status**: ⏳ Pending Test

---

### **TC12: Empty State - No Questions**

**Prerequisites**: No questions in database OR filters return no results

**Expected Results**:
- ✅ Empty state displayed with:
  - Inbox icon
  - Message: "Chưa có câu hỏi nào"
  - "Thêm câu hỏi đầu tiên" button
- ✅ Statistics show 0 for all counts
- ✅ No table displayed

**Current Status**: ⏳ Pending Test

---

## 🔍 Technical Verification

### **TV1: API Integration**

**Check Browser Console (F12)**:
```
Expected logs when loading Question Bank:
- "Fetching all questions from API"
- "Successfully fetched [N] questions"
- "Fetching all subjects from API"
- "Successfully fetched [N] subjects"

Expected logs when creating question:
- "Creating new question for subject: [SUBJECT_ID]"
- "Question created successfully with ID: [QUESTION_ID]"

Expected logs when updating:
- "Updating question with ID: [QUESTION_ID]"
- "Question [QUESTION_ID] updated successfully"

Expected logs when deleting:
- "Deleting question with ID: [QUESTION_ID]"
- "Question [QUESTION_ID] deleted successfully"
```

**Current Status**: ⏳ Pending Test

---

### **TV2: MongoDB Data Verification**

**Use MongoDB Compass or mongosh**:

```bash
# Connect to MongoDB
mongosh "mongodb://localhost:27017/e-testhub"

# View all questions
db.questions.find().pretty()

# Count questions
db.questions.countDocuments()

# Find questions by subject
db.questions.find({ subjectId: "SUBJECT_ID" }).pretty()

# Find questions by difficulty
db.questions.find({ difficultyLevel: "easy" }).pretty()

# Verify last created question
db.questions.find().sort({ createdAt: -1 }).limit(1).pretty()
```

**Current Status**: ⏳ Pending Test

---

### **TV3: Network Requests**

**Check Network Tab (F12 → Network)**:

Expected requests:
1. **GET** `/Teacher/QuestionBank` → 200 OK
2. **GET** `http://localhost:3000/api/questions` → 200 OK
3. **GET** `http://localhost:3000/api/subjects` → 200 OK
4. **POST** `http://localhost:3000/api/questions` → 201 Created
5. **PUT** `http://localhost:3000/api/questions/[ID]` → 200 OK
6. **DELETE** `http://localhost:3000/api/questions/[ID]` → 200 OK

**Current Status**: ⏳ Pending Test

---

## 🐛 Known Issues & Edge Cases

### Issue 1: TeacherController Dependency Injection
**Status**: ✅ Resolved
- Added IQuestionApiService and ISubjectApiService to constructor
- Services registered in Program.cs

### Issue 2: Subject Model ApiId
**Status**: ✅ Resolved
- Subject uses `Id` property, not `ApiId`
- Updated all views to use `subject.Id`

### Issue 3: GetAsync Method Signature
**Status**: ✅ Resolved
- Fixed to use `GetAsync($"questions/{id}")` instead of `GetAsync("questions", id)`

---

## 📊 Test Results Summary

| Test Case | Status | Notes |
|-----------|--------|-------|
| TC1: View Question Bank | ⏳ Pending | |
| TC2: Create Multiple Choice | ⏳ Pending | |
| TC3: Create Essay | ⏳ Pending | |
| TC4: Create True/False | ⏳ Pending | |
| TC5: Form Validation | ⏳ Pending | |
| TC6: Edit Question | ⏳ Pending | |
| TC7: Delete Question | ⏳ Pending | |
| TC8: Filter by Subject | ⏳ Pending | |
| TC9: Filter by Difficulty | ⏳ Pending | |
| TC10: Filter by Type | ⏳ Pending | |
| TC11: Combined Filters | ⏳ Pending | |
| TC12: Empty State | ⏳ Pending | |

**Total**: 0/12 Passed | 0/12 Failed | 12/12 Pending

---

## 🎯 Next Steps After Testing

1. ✅ If all tests pass → Continue with Exam Management (Option B)
2. ❌ If tests fail → Fix issues and retest
3. 💡 If want enhancements → Add Import/Export, Rich Text Editor (Option C)

---

## 📝 Test Notes

**Testing Instructions for User**:

1. **Start MongoDB API first**:
   ```bash
   cd d:\DevProject\CNPMNC\LyThuyet\New\e-testhub
   npm start
   ```

2. **App is already running** at http://localhost:5230

3. **Login with Teacher account** (your existing teacher credentials)

4. **Navigate to**: Dashboard → Ngân hàng câu hỏi (sidebar)

5. **Go through each test case** in order and report results

6. **Check Browser Console** (F12) for any errors or API logs

7. **Report back** with:
   - Which test cases passed ✅
   - Which test cases failed ❌
   - Any error messages
   - Screenshots (if needed)

---

**Bạn đã sẵn sàng test chưa?** 🚀

Hãy:
1. Đảm bảo MongoDB API đang chạy (`npm start` trong e-testhub folder)
2. Navigate đến http://localhost:5230
3. Login với teacher account
4. Vào "Ngân hàng câu hỏi"
5. Test từng test case và báo cáo kết quả! 📋
