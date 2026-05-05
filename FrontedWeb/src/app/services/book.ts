export interface Book {
  id: number;
  title: string;
  author: string;
  price: number;
  image: string;
  description: string;
}

export const BOOKS: Book[] = [
  {
    id: 1,
    title: 'Dế Mèn Phiêu Lưu Ký',
    author: 'Tô Hoài',
    price: 50000,
    image: '/assets/book1.jpg',
    description: 'Truyện thiếu nhi nổi tiếng'
  },
  {
    id: 2,
    title: 'Harry Potter và Hòn đá Phù thủy',
    author: 'J.K Rowling',
    price: 120000,
    image: '/assets/book2.jpg',
    description: 'Phép thuật và phiêu lưu'
  },
  {
    id: 3,
    title: 'Đắc Nhân Tâm',
    author: 'Dale Carnegie',
    price: 85000,
    image: '/assets/book1.jpg',
    description: 'Nghệ thuật giao tiếp và ứng xử'
  },
  {
    id: 4,
    title: 'Tôi Thấy Hoa Vàng Trên Cỏ Xanh',
    author: 'Nguyễn Nhật Ánh',
    price: 75000,
    image: '/assets/book2.jpg',
    description: 'Tuổi thơ dữ dội và đẹp đẽ'
  },
  {
    id: 5,
    title: 'Nhà Giả Kim',
    author: 'Paulo Coelho',
    price: 95000,
    image: '/assets/book1.jpg',
    description: 'Hành trình tìm kiếm ước mơ'
  },
  {
    id: 6,
    title: 'Sapiens: Lược sử loài người',
    author: 'Yuval Noah Harari',
    price: 180000,
    image: '/assets/book2.jpg',
    description: 'Câu chuyện về sự tiến hóa của loài người'
  },
  {
    id: 7,
    title: 'Atomic Habits',
    author: 'James Clear',
    price: 150000,
    image: '/assets/book1.jpg',
    description: 'Xây dựng thói quen tốt, phá bỏ thói quen xấu'
  },
  {
    id: 8,
    title: 'Cây Cam Ngọt Của Tôi',
    author: 'José Mauro de Vasconcelos',
    price: 68000,
    image: '/assets/book2.jpg',
    description: 'Câu chuyện cảm động về tuổi thơ'
  },
  {
    id: 9,
    title: 'Thinking, Fast and Slow',
    author: 'Daniel Kahneman',
    price: 165000,
    image: '/assets/book1.jpg',
    description: 'Tâm lý học về quá trình ra quyết định'
  },
  {
    id: 10,
    title: 'Mắt Biếc',
    author: 'Nguyễn Nhật Ánh',
    price: 72000,
    image: '/assets/book2.jpg',
    description: 'Tình yêu đầu đời trong sáng'
  },
  {
    id: 11,
    title: 'The 7 Habits of Highly Effective People',
    author: 'Stephen R. Covey',
    price: 140000,
    image: '/assets/book1.jpg',
    description: '7 thói quen của người thành đạt'
  },
  {
    id: 12,
    title: 'Tuổi Trẻ Đáng Giá Bao Nhiêu',
    author: 'Rosie Nguyễn',
    price: 89000,
    image: '/assets/book2.jpg',
    description: 'Hướng dẫn cho thế hệ trẻ'
  }
];